## Purpose

While this started as a quick demo to illustrate some ideas about input validation to a dear colleague at NRCan, it quickly evolved into a project that would allow me to practice various object-oriented software design techniques.
Techniques bearing names such as "clean architecture" (i.e. layering), "domain-driven design", "test-driven" etc.
Maybe even a bit of input validation.
It's mainly stuff that I picked up along the years, but never got to put into a clear form.

My hope for this project is twofold:

1. This can be a repository of well-known patterns and ideas, useful for myself and maybe even others;
2. This can give me a bunch of comparison points, to track my future progress as a software developer.

## Concept

This demo is a flashcard service.
More precisely, it's a webservice that allows you to work with flashcards through an API.
Each flashcard represents an atomic piece of information that you want to remember (for instance, one side of the card asks: "Why is the mitochondria known as the powerhouse of the cell?", and the other side of the card answers: "Because it generates most of the cell's supply of adenosine triphosphate").

As your deck of flashcards grows, a major question crops up: how should you choose which card to review?
If you randomly pick cards from your deck, then you're not taking into account the fact you will naturally remember some cards better than other cards.
You'll review easy ones needlessly often; the answers to the more difficult cards you'll forget evermore.
No progress in your study will be made this way.

Some software, such as [Anki](https://apps.ankiweb.net/), solve this problem by using "spaced repetition" techniques.
The ideas are: difficult cards should crop up in reviews more often than easy ones, and the difficulty of a card can change over time.

In this demo, we also solve this problem using spaced repetition techniques.
More precisely, we implement an open-sourced version of a spaced repetition algorithm called FSRS; see the last section of this README for an overview of it, if you care.

## Service Architecture

For this project, I adopted a relatively standard 4-layers architecture.
These are: Core, Application, Api and Infrastructure.

### Core Layer and DDD

The core layer is the highest level of abstraction in our application.

It doesn't care about databases, it doesn't care about IO, it doesn't care about physical time, and it doesn't care about physical space.

In fact, this layer cares about one thing, and one thing only: being the best possible model for the part of reality it cares about.
That part of reality is called the "domain".
In our case, for example, the domain consists roughly of "managing and learning memory units through flashcards".

To be effective, our model needs to be unified, which means it needs to be internally consistent with no contradictions inside within it.
[Here's what Martin Fowler has to say about that](https://martinfowler.com/bliki/BoundedContext.html):
> As you try to model a larger domain, it gets progressively harder to build a single unified model.
> Different groups of people will use subtly different vocabularies in different parts of a large organization.
> The precision of modeling rapidly runs into this, often leading to a lot of confusion.
> Typically this confusion focuses on the central concepts of the domain. [...]
> These subtle polysemes could be smoothed over in conversation but not in the precise world of computers.

Domain-Driven Design (DDD) gives us, among other things, a solution to this problem.
The DDD approach splits up the domain into so-called "bounded contexts", and we make one model per bounded context.
Again from Fowler:
> Different contexts may have completely different models of common concepts with mechanisms to map between these polysemic concepts for integration.

Moreover, while bounded contexts divide the domain, they don't necessarily partition it: different bounded contexts may share domain objects, but model them at different levels of details.
That's very useful!

In our case, however, the domain is so small that we can use a single bounded context that coincides with the domain.
I'll still be using DDD terminology for the fun and practise of it.

#### Entities and Values

Because we're working with C# and because C# is an object-oriented language, our model will have to be made up of objects.
More precisely, there'll be two kinds of objects: entities and values. Here's how to make the distinction:

- An entity always has an id, which gives it an identity.
  The only way to check if two entities are the same is by comparing their identity.
  In other words, the properties of an entity are free to change over time without the entity suddenly becoming different from itself.
  If you change the color of your hair tomorrow, you'll still be the same person as you are today.
  Slogan: 
  > An entity is greater than the sum of its parts.
- A value has no id and is completely determined by its contents.
  It's essentially a glorified tuple, which can most of the time be implemented as a `record` in C#.
  We check that two values are the same by checking if their contents are the same.
  Slogan:
  > A value is equal to the sum of its parts.

#### Domain Rules

Domain rules are constraints which the domain objects should always respect.
Those rules *have* to hold, this is a *model-level* requirement.
Domain rules are independent of the concrete design of the software and exist at the model level.
This is in contradistinction with *application rules*, which lie at a lower abstraction level and exist only because we must choose a concrete design after all, see below.

## Let's Model

Let me pretend for a moment that I'm not the one who wrote the Appendix below about the algorithm.

First things, I read the description of the algorithm.
We're talking about "cards" of course.
Also about "memory units", but it seems to me from prior knowledge about how these kind of systems work "in real life" that everytime we'll talk about a "memory unit" it'll be through the medium of a particular card.

It seems every card has some state related to the algorithm, in particular stability `S`.
I'm thinking S will be what to filter on when implementing the card selection algorithm.

Looks like what happens when a card is first created is not well-defined in the description.
It says what should happen when a card has a difficulty, but which difficulty should the system give to a newly minted card?
There's a sentence saying the user will give an estimated grade, and there's a bunch (3) of equations to determine the initial state.
But how should we select which new card to show to the user, if there's many of them?
I believe randomly is the only answer here, because we have no user information regarding the card, and it's not in scope to estimate a card's difficulty based on its content.

### Business Rules

- Grading a card may only happen chronologically
- Only cards that have been reviewed have a stability
- No two cards in the same deck can share a question (front face)

## Appendix: Quick Look at the FSRS v1 Algorithm

The Free Spaced Repetition Scheduler (FSRS) is one of the spaced repetition algorithms used by Anki, among other similar software.
I'll try to describe FSRS version 1 concisely here; if you want more details about it, you can see [the bottom of this page](https://github.com/open-spaced-repetition/awesome-fsrs/wiki/The-Algorithm) at the official project's GitHub.
At the time of writing, this algorithm exists in versions up to 6, but I'll only detail and implement v1 for simplicity (v6 has 21 parameters while v1 has only 7).

### Three-Component Model of Memory

FSRS is based on the "Three-Component Model of Memory", which tries to describe how a given piece of information fades away from your memory as time goes on.
According to this model, the fading behavior is controlled by 3 variables:

- Retrievability (`R(t)`), the probability that you can recall the unit of information at a given time.
- Stability (`S`), the number of days before `R` decreases from 100% to 90%.
- Difficulty (`D`), the inherent complexity of the unit of information.

Those variables are not independent.
For instance, if a unit of information has a higher difficulty, then its stability will be lower since it'll take you less time to forget its intricacies.
A lower stability will mean lower retrievability at any given time, as well.

We interpret `S(n)` as the minimal number of days we should wait before presenting a flashcard for review.
Ideally, we should not wait longer than that, but that will depend on how many cards the user requests in a session, among other factors.

### State Update Formulae

The v1 of the algorithm has 7 parameters, `w[0]` up to `w[6]`.
Those parameters are weights that can be "machine learned", but the FSRS project helpfully gives us sensible starting values for the parameters:
```json
[2, 5, 3, -0.7, -0.2, 1, -0.3]
```

The memory state is represented by stability `S`, difficulty `D`, and lapses `L` which are the total number of times the card has been forgotten.
The purpose of the update formulae is to determine how to go from a state `(S, D, L)` to the next `(S', D', L')`.
This state transition is fully determined by:

- `t`, the total time elapsed since the last state transition (i.e. since the last review), and
- `G`, the grade, which is user input for how difficult was recall.

Concretely, we'll take the grade `G` as an integer ranging from 1 to 4 (inclusive).
1 will be taken to means "again", which signifies the user completely failed to recall the information; then 2, 3, 4 signify "hard", "good", "easy" respectively.

When seeing a card for the first time, the user will give it an estimated grade `G`.
Then, the initial memory state is given by:

- Initial stability: `S = w[0] * 0.25 * 2^(G - 1)`
- Initial difficulty: `D = w[1] - (G - 3)`
- Initial lapses: `L = 1` if `G = 1` (i.e. graded "again"), otherwise `L = 0`.

A memory state update for a card happens every time the user reviews the card.
First, we must figure out how the retrievability changed for that card, since it depends on `t` the time that elapsed since the last review.
For this we use the equation `R = 0.9^(t/S)`, where `S` is the current stability for the card.
While the retrievability is not directly part of the memory state, it does take an important part in the state transition as it's the only time-related component of the computation.

The difficulty `D` is a real number which changes depending not only on the user-given grade `G` but also on the current difficulty and retrievability.
To update it, we use the update formula `D' = max(0, D + R - 0.25 * 2^(G - 1) + 0.1)`.
The unprimed variables here represent current values.

Next, we compute the updated stability `S'`. How we do that depends on the user's input (the grade).
After a successful review (grading is "hard", "good" or "easy", meaning `G` between 2 and 4 inclusive), the new stability is given by `S' = S*(1+exp(w[2])*(D'+0.1)^w[3]*S^w[4]*(exp((1-R)*w[5])-1))`.
In case the review was failed (grading is "again", meaning `G = 1`), the new stability is instead `S' = w[0] * exp(w[6] * L)`.

Finally, the lapses `L` is updated as expected: increase it by 1 if the user graded the card as "again"; otherwise keep it untouched.

### State Update Examples

| Again       | Hard (+1 day)   | Good (+1 day)   | Good (+1 day)   |
|-------------|-----------------|-----------------|-----------------|
| (0.5, 7, 1) | (1.09, 7.41, 1) | (1.59, 7.42, 1) | (2.06, 7.46, 1) |

| Again       | Hard (+1 day)   | Good (+1 day)   | Again (+5 day)  |
|-------------|-----------------|-----------------|-----------------|
| (0.5, 7, 1) | (1.09, 7.41, 1) | (1.59, 7.42, 1) | (3.79, 7.99, 1) |

### Notes

Navigated a lot of shit before this https://github.com/open-spaced-repetition/ts-fsrs/blob/main/packages/fsrs/src/algorithm.ts

I used this [visualizer](https://open-spaced-repetition.github.io/anki_fsrs_visualizer) and copied the Stability table into my tests.
The goal was to check if my calculations matched with the visualizer's results.
Reasons why I wasn't getting the right numbers for a while:
- The visualizer grades a card after `min(1, round(stability))` days, which is (i) an integer amount of days, and (ii) always at least 1 day.
  I was using exactly the stability as the amount of (fractional) days before the next grading.
- The visualizer's code uses [this TypeScript library](https://github.com/open-spaced-repetition/ts-fsrs),
  and it took me a while to notice all the rounding this library is doing while calculating (see the [algorithm's implementation](https://github.com/open-spaced-repetition/ts-fsrs/blob/main/packages/fsrs/src/algorithm.ts)).