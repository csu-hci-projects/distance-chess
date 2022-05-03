# distance-chess
## Tangible Augmented Reality (TAR) Chess Game
- Andreas Johnson
- Isaac Mount

# Instructions for Running Application
## Setup
First, you will need to install [Unity](https://unity.com/).
Guides for this can be found on their website, as well as in various locations online.

Next, clone this repo into a convenient location on your PC.
Ensure that all required additional packages and modules are installed for Unity, including Android/iOS build support (depending on the device you wish to use) and the following packages:
- AR Foundation (not used, but imported in current version)
- ARCore XR Plugin (for iOS) (not used, but imported in current version)
- ARKit XR Plugin (for Android) (not used, but imported in current version)
- Vuforia Core Samples

In order to use Vuforia, you must check the Vuforia configuration settings.
In the Unity editor, select the `ARCamera` object from the scene hierarchy, then, under the `Vuforia Behaviour (Script)` Component in the Inspector Panel, select `Open Vuforia Engine Configuration`.

![image](https://user-images.githubusercontent.com/34222063/166340121-52ef5822-9aa0-46de-86bb-dd1fed4fa3f3.png)
![image](https://user-images.githubusercontent.com/34222063/166340180-c02cbb79-c96a-4354-9de8-d267f8a5fa1c.png)
Ensure there is a value for the `App License Key` field; if not included, you can add one for free from the Vuforia Developer Portal.
Next, ensure there is a camera device (or your own recorded video).
To do this, in the `Vuforia Engine Configuration` panel, scroll to the bottom and find the field `PlayMode Type`, under the section `Play Mode`.
This is the video feed input for the AR to be rendered over.
For this experiment, we used a webcam, in which we selected `WEBCAM` for `PlayMode Type` and selected the proper device, which should be automatically detected by the Vuforia Engine.

![image](https://user-images.githubusercontent.com/34222063/166340643-54efac8e-fe3f-4b07-b1b9-d11089ab3265.png)

Now, we need to ensure that the `ImageTarget` is set up appropriately.
As above, select the `ImageTarget` object from the scene hierarchy to open the Unity inspector panel.
In the `Image Target Behaviour (Script)` component, please ensure that `From Image` is selected for the field `Type`, and that `Assets/QR-A.jpg` is the image being used.
Under the `Advanced` section, please ensure that the field `Physical Width (m)` has the value `0.127` (5 inches).
See screenshot below.

![image](https://user-images.githubusercontent.com/34222063/166340970-4b70ebdf-9c58-45c5-8fa2-2ba3840fc9ba.png)

To test if Vuforia is working properly, simply click the play button in the Unity editor; if you start getting a camera feed, the configuration is complete and correct.

## Experiment

![image](https://user-images.githubusercontent.com/34222063/166344617-3c08ed92-39b3-4120-bbd5-d159072b7a0b.png)

### Experiment Setup & Configuration
Now, select the `Board` object from the scene hierarchy.
In the inspector panel, you can select which color is being played by the subject.
Only pieces of the other color will be rendered in the scene (as the subject has their own physical set).
In the `Transform` component, you can adjust the size of the board being played upon.
As a default, the value is 12.5in x 12.5in.
If the board you wish to test this on has 1-inch tiles, replace the `Scale` values with X`8` Y`8` Z`8`, or X`16` Y`16` Z`16` for 2-inch tiles, etc.
This scales all the pieces being rendered, as well as the "abstract tiles" used by the application for placing the pieces.

Once you press play, ensure that the QR code is visible in the camera feed, and adjust the camera as necessary to include visibility for the entire physical board.
Reposition the QR-code printout so that the pieces line up with the tiles on the physical board.
Great!
You are now done with all the setup and can begin rendering moves.

### Running the Experiment
Explain to the subject that the pieces being rendered move upon input from the experimenter.
Explain that the experimenter will be making moves on behalf of a computer engine opponent.
This opponent should be chosen based on the self-proclaimed skill of the subject.
For beginners, we chose the Chess.com engine "Aron", under the beginner computer opponents (Elo 700).
For intermediate and advanced subjects, we let them specify which Elo they would like to play against.
As we are comparing performance between interfaces, the difficulty of the engine should match the player fairly well, but does not need to be uniform across subjects.

To actually have the subject play the game, after selecting the difficuly of the computer opponent, the experimenter will play the subject's moves against the computer opponent using Chess.com, then enter the moves of the computer opponent on the experiment application.

#### Entering the Computer's Moves
To enter the computer's moves, the experimenter should have the inspector panel open for the `Board` object during execution.
Then, given a computer move from Chess.com, the experimenter will enter a PGN-like value into the `Engine Moves` field.
For accuracy, the experimenter can see the current registered move in the field `Engine Move`.
Deleting the previous values in `Engine Moves` field is not required.
The application will simply grab the move at the end of the string.

We chose to modify PGN for ease of coding and entry.
- Pawn Moves: for simple forward moves, use the format `<pawn file><rank to move to>`, such as `e4` for moving the pawn on `e2` to `e4`. For captures, rather than classic PGN, which for example would be `exd5` for pawn on `e4` capturing on `d5`, it is simply `<capturing pawn file><captured tile>`, such as `ed5` for the previous example. For promotions, it is simply `<pawn file>=<piece type>` for forward promotions, such as `e=q` for the pawn on `e7` moving to `e8` and promoting to a queen. For capture-promotions, use the format `<capturing pawn file><captured file>=<piece type>`, such as `ed=q` for the pawn on `e7` to capture on `d8` and promoting to a queen.
- Back-Rank Piece Moves: since there are no pieces of the other color, simply use the format `<piece type><tile to move to>`, or for ambiguous moves (such as a knight move to `d2` with knights on `b1` and `f3`), use the format `<piece type><current piece file, rank, or both><tile to move to>`. So for example, entering `nf3` will find the knight that can move to this tile and perform the move. However, if there are multiple knights that can make this move, it will not be made. Instead, use `ngf3` or `n1f3`, and the correct knight will be moved. In the rare case of there being 3 pieces able to make the move *and* making a right triangle (2 pieces share the same file and 2 pieces share the same rank), both file and rank are needed for the move, such as `qb3b6`. This would be necessary if there were three queens able to reach `b6` and making a right triangle with each other, such as in positions `b3`, `b7`, and `d5`. The `piece type` letters are shown below.
  - Rook: `r`
  - Knight: `n`
  - Bishop: `s` (couldn't be b, as there could be ambiguity between a pawn capture and a bishop move; in classic PGN, the bishop is denoted with B, but we chose s simply to make the input simpler and faster)
  - Queen: `q`
  - King: `k`
  - NOTE: note that these are the same characters used for pawn promotion. So, if (for some strange reason) the chess engine decided to promote the e-pawn to a bishop, it would be entered as `e=s`, not `e8=B`.
- Castling: to castle long (queenside), use `ooo`, and to castle short (kingside), use `o o`. Note these are lower-case o's, and that to castle short there is a single space between two o's.

#### Capturing the Computer's Pieces
When the subject captures a computer's piece, use the notation `-<position>`, such that if the subject, playing as white, captured the computer's d-pawn (`d5`) with their e-pawn (`e4`), classic PGN would list the move as `exd5`.
However, all we need to know is that the pawn on `d5` should no longer be rendered.
So, the experimenter would simply enter `-d5`, where it is a single dash followed by the position of the piece to be removed.

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
- [x] Piece move scripting
  - [x] animations
  - [x] pawns
  - [x] kings
  - [x] rooks
  - [x] knights
  - [x] bishops
  - [x] queens
- [ ] `Board` scripting
  - [x] piece positions
  - [x] attacks on tiles
  - [ ] checks & checkmates (in progress)
  - [ ] stalemates
  - [ ] pins on pieces
  - [x] castling
  - [x] captures (in progress)
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

### [Vuforia Core Samples](https://assetstore.unity.com/packages/templates/packs/vuforia-core-samples-99026) by [PTC Vuforia](https://developer.vuforia.com/)
This package is used for the AR rendering of the computer opponent's pieces.
Due to package size, this could not be pushed to the repo.
So, you will have to manually install the package, using the Package Manager within the Unity editor.
There are plenty of resources available online to help do this, and ensure the project is configured properly by following the steps in the 'Setup' section.

## Other Software
### [Lucas Chess](https://lucaschess.pythonanywhere.com/) by [lukasmonk](https://github.com/lukasmonk) ([GitHub Repo](https://github.com/lukasmonk/lucaschess))
A chess analysis toolkit which we will use to generate engine moves to play against participants.
We will also use this software to analyze games for particpants' cognitive performance.
