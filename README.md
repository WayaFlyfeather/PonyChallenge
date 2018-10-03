# PonyChallenge
### (aka Pony Tjalnz)

This is a solution for the [My Little Code Challenge as posed by Trustpilot](https://ponychallenge.trustpilot.com/index.html). In short, the challenge is to use a web service provided by Trustpilot to create a maze, and using the service to move the pony to safety, avoiding the domokun.

(The description of the challenge and the service is included in the project, in case the link should be taken down at some point, as PDFs: the [challenge description](PonyChallenge/PonyChallenge/ChallengeDescription/SaveThePony.pdf) and the [API](PonyChallenge/PonyChallenge/ChallengeDescription/PonyAPI.pdf).)

From a challenge perspective, the important parts are probably in consuming the web service, and the algorithm that determines the best move for the pony. These parts are to be found in the [HTTPPonyMazeService](PonyChallenge/PonyChallenge/Services/HTTPPonyMazeService.cs) and [MazeNavigator](PonyChallenge/PonyChallenge/MazeNavigator.cs) classes.

Building a Xamarin App for both Windows, Android and iOS on top of that is probably a bit over the top, but was also fun. And it was the first time I used SkiaSharp, which was a pleasant experience.

A note on this repo: the main branch is named `main` instead of `master`, to avoid it being indexed by Google.
