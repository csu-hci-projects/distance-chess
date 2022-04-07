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
  - [ ] occlusion
- [ ] Piece move scripting
  - [ ] animations (in progress)
  - [x] pawns
  - [ ] kings
  - [ ] rooks
  - [ ] knights
  - [ ] bishops
  - [ ] queens
- [ ] Board scripting
  - [x] piece positions
  - [ ] attacks on tiles (in progress)
  - [ ] checks & checkmates (in progress)
  - [ ] stalemates
  - [ ] pins on pieces (in progress)
  - [ ] captures (in progress)
  - [ ] en passant captures
  - [ ] pawn promotions
- [ ] Computer Vision
  - [ ] recognizes pieces
  - [ ] recognizes moves
  - [ ] checks for captures on player's pieces
  - [ ] highlights legal move tiles when player picks up piece

# Outside Resources
## Unity Software
### [Free Little Games Asset Pack](https://assetstore.unity.com/packages/3d/props/free-little-games-asset-pack-125089#publisher) by [Ferocious Industries](https://assetstore.unity.com/publishers/37734)
We will use the 3D models & materials for a chess set and board for rendering in the AR application.

## Other Software
### [Lucas Chess](https://lucaschess.pythonanywhere.com/) by [lukasmonk](https://github.com/lukasmonk) ([GitHub Repo](https://github.com/lukasmonk/lucaschess))
A chess analysis toolkit which we will use to generate engine moves to play against participants.
We will also use this software to analyze games for particpants' cognitive performance.
