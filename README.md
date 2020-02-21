# WPF Chess

This is a chess game based on .NET Core 3 and the WPF framework.
No other frameworks, repositories or nuget packages were used.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for fun and testing purposes. 

## Deployment

Just start the thing. If you want to play a fresh game without any custom starting board, simply change the path in class **_Game.cs_** to the **_Structure.xml_**. If you have added a custom starting board, simply change the path:
```
var board = Serializer.FromXml<Board>(@"..\..\..\..\Library\Xml\Structure.xml");
```
### Representation of the chess board

The chess board is represented as a list of the object **_Square_** ([x,y]). Each **_Square_** has a unique Point with coordinates.
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
### Make a custom chess board
You can make a custom chess board. Here you can change the starting formation of the pieces as you want without touching the code. Just add a XML file to the folowing location:
```
Chess\Library\Xml\
```
The XML construction plan is presented in the empty board file without any pieces. Feel free to make a copy and generate your own endgame situation by adding the pieces at the prefered location:
```
Chess\Library\Xml\Empty.xml
```

The pieces can be added to the xml in the following way.<br/>
Just add the specific piece tag to the square tag you want and define the color of the piece.<br/>
**Important!** Don't change the number of squares inside the XML, it should should always be 64, otherwise it wouldn't be chess.
```
<Square>
  ...
  <King>
    <Color>White</Color>
  </King>
</Square>
```
The following tag names are available.

| Pieces   | 
| ---------| 
| King     |
| Queen    |  
| Bishop   | 
| Knight   | 
| Pawn     | 
| Rook     | 

## Authors

* **Dominik Weder** - *Main* - [dodoweder97](https://github.com/dodoweder97)
