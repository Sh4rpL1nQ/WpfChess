# WPF Chess

This is a chess game based on .NET Core 3 and the WPF framework.<br/>
You can generate custom piece formations and can change all important settings (e.g. maximum time for the players) outside of the application code (XML file). 

## Getting Started

These instructions will get you a copy of the project and let it run on your local machine for fun and testing purposes. 

## Table of Contents  
[Deployment](#deploy)  
[Main Settings](#settings) <br/>
[Make a custom chess board](#custom) <br/>
[Load and export chess board](#xml) <br/>
[Important notes](#important)

<a name="deploy"/>

## Deployment

In order to start a fresh game without any custom piece formation, simply start the _.exe_ file.

<a name="settings"/>

## Main Settings

The main settings are changeable via XML file. The path to this **_Settings.xml_** is:
```
..\Chess\Library\Xml\
```
The different settings are:
| Tag   | Description |
| --------| --------|
| Priority  | Name of the TXT file to be loaded, which contains the information about the chess board and its pieces. This file will be loaded, even if the game is over and the formation should switch. |
| Switch1    | Name of the TXT file to be loaded, which contains the information about the chess board and its pieces. This file will be loaded at first, but only if _Priority_ is not set. Then it alternates with the value of _Switch2_ after the game is over.   |
| Switch2   | Name of the TXT file to be loaded, which contains the information about the chess board and its pieces. This file will be loaded at second, but only if _Priority_ is not set. Then it alternates with the value of _Switch1_ after the game is over.   |  
| PlayerTimeInMinutes  | Sets the maximum amount of time every player is allowed to spend on their whole game. No point numbers allowed.  |

<a name="custom"/>

## Make a custom chess board (custom location of all pieces)
In the following parts the generation of a custom board is explained. <br/>

<a name="general"/>

### General Information

The chess board is represented as a list of the object **_Square_** ([x,y]). Each **_Square_** has a unique **_Point_** with coordinates.
```
[0,0] [1,0] [2,0] [3,0] [4,0] [5,0] [6,0] [7,0]
[0,1] [1,1] [2,1] [3,1] [4,1] [5,1] [6,1] [7,1]
...
...
...
...
...
[0,7] [1,7] [2,0] [3,0] [4,0] [5,0] [6,0] [7,0]
```
Based on the cooardinates you can set the prefered piece.

### Setup
If you want to generate a custom board, make a copy of **_Empty.txt_** inside the XML path (board structure without pieces) and rename it to whatever you want. Don't forget to to change the _Priority_ in the settings XML file **_Settings.xml_**, otherwise it will still load the default board based on _Switch1_. <br/>
There are three different other TXT files which you can use:
| Number in TXT   | LookupTable-Value |
| --------| --------| 
| Start_White    | Normal starting piece formation with White at the top.  |
| Start_Black   | Normal starting piece formation with Black at the top.  |  
| PromotionsTest  | Test board to show working mechanism of promoting a Pawn.  |

### Construction
How the TXT must be build up.

| Part     | Description |
| --------- | -------------------- |
| 1     | [Setting the moving direction of each piece](#part1) |[Setting the color of each piece](#important)
| 2     | [Setting the type and location of the pieces](#part2) |
| 3     | [Setting the color of each piece](#part3) |

<a name="part1"/>

#### Which color do the top and bottom pieces have
The first two values represent the order of the colors. 
| TXT Value     | Piece Color |
| --------- | -------------------- |
| White     | White |
| Black     | Black |
<br/>
The first color corresponds to the top and the second color corresponds to the bottom pieces. <br/>
This is importand in case of the Pawns, because they can only move in the direction of the enemy. Based on the starting side (top, bottom) they have different directions where they can move to.
Here an example for a potential starting board.

```
white
black
```
This means the pieces (Pawns) which will have the color white, will move to the direction of positive y coordinate (from top downwards) and those with the color black, will move to the direction of negativ y coordinate (from bottom upwards).

<a name="part2"/>

#### Where are the pieces located and what type of piece is it

This 8x8 matrix represents the chess board described in the [General Settings](#general).<br/>
The type of piece corresponds to an integer value. You can find them inside the **_LookupTable.txt_**.
| Number in TXT   | LookupTable-Value |
| --------| --------| 
| Rook    | 1    |
| Knight   | 2   |  
| Bishop  | 3  |
| Queen  | 4  |
| King    | 5    | 
| Pawn    | 6    | 
| No piece    | 0    | 

As an example I extracted the matrix of the normal starting board out of the **_Start_White_** with White at the top.

```
1 2 3 5 4 3 2 1
6 6 6 6 6 6 6 6
0 0 0 0 0 0 0 0
0 0 0 0 0 0 0 0
0 0 0 0 0 0 0 0
0 0 0 0 0 0 0 0
6 6 6 6 6 6 6 6
1 2 3 5 4 3 2 1
```

<a name="part3"/>

#### What color do the pieces have?

The first two colors in our TXT are responsible to deklare the moving direction of each piece.
Now we have to assign the colors of our choice to every piece. <br/>
Therefore riht below our 8x8 matrix, there is another 8x8 matrix for the assigned colors. Every entry with a value greater than 0 must have a number 1 or 2.

| Number in TXT     | Description |
| --------- | -------------------- |
| 1     | Corresponds to the color which was declared first |
| 2     | Corresponds to the color which was declared second |

To complete the example, here is the color assignment of a potential starting board.

```
1 1 1 1 1 1 1 1
1 1 1 1 1 1 1 1
0 0 0 0 0 0 0 0
0 0 0 0 0 0 0 0
0 0 0 0 0 0 0 0
0 0 0 0 0 0 0 0
2 2 2 2 2 2 2 2
2 2 2 2 2 2 2 2
```

<a name="xml"/>

## Save your current board
In addition to load a prebuild txt file, you are now able to load already played (and not finished) boards via XML file.
To archieve this, simply choose "Options" in the menu bar and click on _Export Board (XML)_. Then you can name your file and put it everywhere you want. <br/><br/>
When you've saved the current board state into the XML file, you can can continue playing it, whenever you want, by pressing the _Import Board (XML)_ option in the top menu. Here you can select your XML to be deserialized into the board object.

<a name="important"/>

### Important notes
**_Most Important:_** There has to be ALWAYS one King of each color, otherwise the game would't make sense
1. Do not change the amount of numbers in the TXT. Each chess game has 8x8 fields.<br/>
2. Do not write other colors than black or white. Each chess game has just black and white pieces.

## Authors

* **Dominik Weder** - *Main* - [Sh4rpL1nQ](https://github.com/Sh4rpL1nQ)
