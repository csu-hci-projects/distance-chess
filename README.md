# distance-chess
## Tangible Augmented Reality (TAR) Chess Game
- Andreas Johnson
- Isaac Mount

# Project Overview
## Current Progress
- [ ] Rendering of pieces
  - [x] basic piece and board rendering
  - [ ] only renders opponents' pieces
  - [ ] occlusion
- [ ] Piece move scripting
  - [x] pawns
  - [ ] kings
  - [ ] rooks
  - [ ] knights
  - [ ] bishops
  - [ ] queens
- [ ] Board scripting
  - [x] records piece positions
  - [x] tracks pins on pieces
  - [x] tracks attacks on tiles for allowed king movement
  - [ ] en passant captures
- [ ] Computer Vision
  - [ ] recognizes pieces
  - [ ] recognizes moves
  - [ ] checks for captures on player's pieces
  - [ ] highlights legal move tiles when player picks up piece

# Outside Resources
## Software: Unity
### [Free Little Games Asset Pack](https://assetstore.unity.com/packages/3d/props/free-little-games-asset-pack-125089#publisher) by [Ferocious Industries](https://assetstore.unity.com/publishers/37734)
We will use the 3D models & materials for a chess set and board for rendering in the AR application.

## Software: Other
### [Lucas Chess](https://lucaschess.pythonanywhere.com/) by [lukasmonk](https://github.com/lukasmonk) ([GitHub Repo](https://github.com/lukasmonk/lucaschess))
A chess analysis toolkit which we will use to generate engine moves to play against participants.
We will also use this software to analyze games for particpants' cognitive performance.
