## Concept

This demo is a flashcard service.
More precisely, it's a web service that allows you to work with flashcards through an API.
Each flashcard presents an atomic piece of information that you want to remember, often in the form of an answer to a question (for instance, one side of the card asks: "Why is the mitochondria known as the powerhouse of the cell?", and the other side of the card answers: "Because it generates most of the cell's supply of adenosine triphosphate").

Hence, a major part of this software consists of a simple CRUD for flashcards, or decks of them.
But that's not all.

As a deck of flashcards grows, a major question crops up: how should you choose which card to review?
If you randomly pick cards from your deck, then you're not taking into account the fact you will naturally remember some cards better than other cards.
You'll review easy ones needlessly often; the answers to the more difficult cards you'll forget evermore.
No progress in your study will be made this way.

Some software, such as [Anki](https://apps.ankiweb.net/), solve this problem by using "spaced repetition" techniques.
The ideas are: difficult cards should crop up in reviews more often than easy ones, and the difficulty of a card can change over time.

In this demo, we also solve this problem using spaced repetition techniques.
More precisely, we implement an open-sourced version of a spaced repetition algorithm called FSRS;
see the following links for details:
- [TypeScript implementation](https://github.com/open-spaced-repetition/ts-fsrs/blob/52d24e84cb5dc85d87852ada48648c01e6a20246/packages/fsrs/src/algorithm.ts)
- [FSRS visualizer](https://open-spaced-repetition.github.io/anki_fsrs_visualizer) (that I used for getting test values)
- [Official algorithm documentation](https://github.com/open-spaced-repetition/awesome-fsrs/wiki/The-Algorithm)
- [Alternative doc](https://expertium.github.io/Algorithm.html)
- At the end of this file, I tried to summarize the underlying mathematical model used by FSRS.
  You can also find a more expansive description in [the official docs](https://github.com/open-spaced-repetition/awesome-fsrs/wiki/Spaced-Repetition-Algorithm:-A-Three%E2%80%90Day-Journey-from-Novice-to-Expert#the-three-component-model-of-memory).

## Appendix: Three-Component Model of Memory

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

### Notes

I used this [visualizer](https://open-spaced-repetition.github.io/anki_fsrs_visualizer) and copied the Stability table into my tests.
The goal was to check if my calculations matched with the visualizer's results.
Reasons why I wasn't getting the right numbers for a while:
- The visualizer grades a card after `min(1, round(stability))` days, which is (i) an integer amount of days, and (ii) always at least 1 day.
  I was using exactly the stability as the amount of (fractional) days before the next grading.
- The visualizer's code uses [this TypeScript library](https://github.com/open-spaced-repetition/ts-fsrs),
  and it took me a while to notice all the rounding this library is doing while calculating (see the [algorithm's implementation](https://github.com/open-spaced-repetition/ts-fsrs/blob/main/packages/fsrs/src/algorithm.ts)).
  I wonder if the rounding is what was causing the tiny discrepancies between my tests results and the visualizer's stability table?