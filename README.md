# distance-chess
## Tangible Augmented Reality (TAR) Chess Game
- Andreas Johnson
- Isaac Mount

# Project Overview
The goal of this project is to explore how interfaces can affect users' cognitive performance.
Chess offers an ideal task for testing this, being widely available in many interfaces.
We will be testing a tangible augmented reality (TAR) interface and a digital interface.
This repo is the code base for the TAR version of chess, while the digital version will be played on Chess.com.

The end goal for this repo is to have a TAR chess application which recognizes a player's moves on a physical board, while rendering the moves from a digital opponent via the AR pieces.
This will primarily be built for iOS, but future builds could include Android.

## Current Progress
- [ ] Rendering of pieces
  - [x] basic piece and board rendering
  - [ ] only renders opponents' pieces (in progress)
  - [ ] occlusion (may not be implemented)
- [ ] Piece move scripting
  - [x] animations
  - [x] pawns
  - [x] kings
  - [x] rooks
  - [x] knights
  - [x] bishops
  - [ ] queens (in progress)
- [ ] `Board` scripting
  - [x] piece positions
  - [ ] attacks on tiles (in progress, missing queen attacks)
  - [ ] checks & checkmates (in progress)
  - [ ] stalemates
  - [ ] pins on pieces
  - [x] castling
  - [ ] captures (in progress)
  - [ ] en passant captures
  - [ ] pawn promotions
- [ ] Computer Vision (may not be implemented)
  - [ ] recognizes pieces
  - [ ] recognizes moves
  - [ ] checks for captures on player's pieces
  - [ ] highlights legal move tiles when player picks up piece

The reason computer vision is marked as "may not be implemented" is due to the scope of the project.
If time permits, we will integrate computer vision into the application; however, if this is not the case, we may need to implement a developer interface which sidesteps the computer vision aspect.
This would be the "Wizard of Oz" solution, in which we would tell the experiment's participants that the application recognizes their moves, while in reality the experimenters would be manually entering the participant's moves.
This will already be done for the chess engine's moves, so the interface for the experimenters will already be present.

# Outside Resources
## Unity Software
### [Free Little Games Asset Pack](https://assetstore.unity.com/packages/3d/props/free-little-games-asset-pack-125089#publisher) by [Ferocious Industries](https://assetstore.unity.com/publishers/37734)
We will use the 3D models & materials for a chess set and board for rendering in the AR application.

## Other Software
### [Lucas Chess](https://lucaschess.pythonanywhere.com/) by [lukasmonk](https://github.com/lukasmonk) ([GitHub Repo](https://github.com/lukasmonk/lucaschess))
A chess analysis toolkit which we will use to generate engine moves to play against participants.
We will also use this software to analyze games for particpants' cognitive performance.
