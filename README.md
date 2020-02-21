# WPF Chess

This is a chess game based on .NET Core 3 and the WPF framework.<br/>
No other frameworks, repositories or nuget packages were used.</br>
You can generate custom piece formations and can change all important settings (e.g. time) outside of the application code (XML). 

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for fun and testing purposes. 

## Table of Contents  
[Deployment](#deploy)  
[Change player time](#time) <br/>
[Make a custom chess board](#custom) 

<a name="deploy"/>
## Deployment

Just start the thing. If you want to play a fresh game without any custom starting board, simply change the tag _BoardXmlName_ in the settings XML file **_Settings.xml_** (default) to **Structure**. (That will guide the application to use the **_Structure.xml_** file inside the XML folder)
The location of all XML files (Settings.xml, Structure.xml, all custom boards) are saved under the path:
```
..\Chess\Library\Xml\
```
<a name="time"/>
## Change time the players have to play the game
Simply change the tag _PlayerTimeInMinutes_ in the settings XML file **_Settings.xml_**

<a name="custom"/>
## Make a custom chess board (custom location of all pieces)
In the following parts the generation of a custom board is explained. <br/>

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
Based on the cooardinated you can set the piece you want.

### Setup
If you want to generate a custom board, make a copy of **_Empty.xml_** inside the XML path (board structure without pieces) and rename it to whatever you want. Don't forget to to change the _BoardXmlName_ in the settings XML file **_Settings.xml_**, otherwise it will still load the default board.

### Add pieces
The pieces can be now added to your xml in the following way.<br/>
Just add the specific piece tag to the square tag you want and define the color of the piece.<br/>

```
<Square>
  ...
  <King>
    <Color>White</Color>
  </King>
</Square>
```
**Important!** Don't change the number of squares inside the XML, it should should always be 64, otherwise it wouldn't be chess.<br/><br/>

The following tag names are available.

| Piece   | XML Tag |
| --------| --------| 
| King    | King    |
| Queen   | Queen   |  
| Bishop  | Bishop  |
| Knight  | Knight  |
| Pawn    | Pawn    | 
| Rook    | Rook    | 
<br/>

Because it's chess, the color of the pieces can only be black or white.<br/><br/>
**Important!** Don't change the color the something else. The values will be deserialized into an enum of only black and white, not into the System.Drawing.Color type.

| Color     | XML Tag with content |
| --------- | -------------------- |
| White     | <Color>White</Color> |
| Black     | <Color>Black</Color> |

## Authors

* **Dominik Weder** - *Main* - [dodoweder97](https://github.com/dodoweder97)
