using Library;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Chess.ViewModels
{
    public class ApplicationViewModel : PropertyChangedBase
    {
        #region Private Fields
        private GameOverViewModel gameOverModel;
        private ApplicationSettings settings;
        private Square selectedSquare;
        private string settingsPath = DirectoryInfos.GetPath("Settings.xml");
        #endregion

        #region Constructors
        public ApplicationViewModel()
        {
            settings = Serializer.FromXml<ApplicationSettings>(settingsPath);

            Chess = new Game(settings);

            Chess.Board.OnInitiatePawnPromotion += Board_OnInitiatePawnPromotion;
            Chess.Board.OnCastlePossible += Board_OnCastlePossible;
            Chess.Board.OnPieceCaptured += Board_OnPieceCaptured;

            PlayerModel1 = new PlayerViewModel(Chess.Board, Color.White, settings.PlayerTimeInMinutes);
            PlayerModel2 = new PlayerViewModel(Chess.Board, Color.Black, settings.PlayerTimeInMinutes);
            PlayerModel1.Player.UserName = "Player1";
            PlayerModel2.Player.UserName = "Player2";

            PlayerModel1.Player.OnGameOver += Player_OnGameOver;
            PlayerModel2.Player.OnGameOver += Player_OnGameOver;
            PlayerModel1.TimeIsOver += Player_OnGameOver;
            PlayerModel2.TimeIsOver += Player_OnGameOver;

            PlayerModel1.OnPromotionSelected += PlayerModel_OnPromotionSelected;
            PlayerModel2.OnPromotionSelected += PlayerModel_OnPromotionSelected;

            ImportTxtCommand = new ActionCommand(ImportTxtAction);
            ImportXmlCommand = new ActionCommand(ImportXmlAction);
            ExportXmlCommand = new ActionCommand(ExportXmlAction);

            SquareCommand = new ActionCommand(SquareAction);

            SetTurnedPlayer();
        }
        #endregion

        #region Properties
        public Game Chess { get; set; }

        public PlayerViewModel PlayerModel1 { get; set; }

        public PlayerViewModel PlayerModel2 { get; set; }

        public GameOverViewModel GameOverModel
        {
            get { return gameOverModel; }
            set { RaisePropertyChanged(ref gameOverModel, value); }
        }

        public PlayerViewModel PlayerAtTurn => PlayerModel1.Player.IsMyTurn ? PlayerModel1 : PlayerModel2;

        public PlayerViewModel PlayerAtWait => PlayerModel1.Player.IsMyTurn ? PlayerModel2 : PlayerModel1;

        public ICommand ImportTxtCommand { get; }

        public ICommand ImportXmlCommand { get; }

        public ICommand ExportXmlCommand { get; }

        public ICommand SquareCommand { get; }
        #endregion

        #region Events and Event-Methods
        private void Board_OnInitiatePawnPromotion(object sender, EventArgs e)
        {
            PlayerAtTurn.ReceivingPiece = sender as Piece;
        }

        private void Board_OnPieceCaptured(object sender, EventArgs e)
        {
            PlayerAtWait.Player.LostPieces.Add(sender as Piece);
        }

        private void Board_OnCastlePossible(object sender, EventArgs e)
        {
            var res = Message.StartBox(Level.Question, "Do you want to castle?", "Castle possible");
            if (res == System.Windows.MessageBoxResult.Yes)
                PlayerAtTurn.Player.ExecuteCastle(sender as List<Square>);
        }

        private void Player_OnGameOver(object sender, EventArgs e)
        {
            var looser = sender as Player;
            var winner = looser.Color == PlayerModel1.Player.Color ? PlayerModel2.Player : PlayerModel1.Player;
            GameOverModel = new GameOverViewModel(winner, looser, (e as GameOverEventArgs).GameOver);
            PlayerModel1.StopTimer();
            PlayerModel2.StopTimer();
            GameOverModel.OnRetry += GameOverModel_OnRetry;
        }

        private void PlayerModel_OnPromotionSelected(object sender, EventArgs e)
        {
            Chess.Board.Squares.FirstOrDefault(x => x.Point.Equals(PlayerAtTurn.ReceivingPiece.Point)).Piece = sender as Piece;
            PlayerAtTurn.ReceivingPiece = null;
            ChangeTurns();
        }

        private void GameOverModel_OnRetry(object sender, EventArgs e)
        {
            GameOverModel = null;
            Chess.Reset();
            PlayerModel1.Reset();
            PlayerModel2.Reset();
            SetTurnedPlayer();
        }
        #endregion

        #region Public Methods
        public void SquareAction(object sender)
        {
            var square = (sender as Square);

            if (selectedSquare != null && !square.Point.Equals(selectedSquare.Point) && square.Piece?.Color != selectedSquare.Color)
            {
                if (PlayerAtTurn.Player.Move(selectedSquare.Piece, square))
                    if (PlayerAtTurn.ReceivingPiece == null)
                        ChangeTurns();

                ClearSelections();

                return;
            }

            if (square.Piece == null) return;

            ClearSelections();

            if (square.Piece.Color == PlayerAtTurn.Player.Color)
            {
                square.IsSelected = true;
                selectedSquare = square;
                Chess.Board.ShowPossibleMoves(selectedSquare.Piece);
            }
        }

        public void ImportTxtAction(object sender)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Choose the TXT file to be imported";
            dialog.Filter = "Text|*.txt|All|*.*";

            if (dialog.ShowDialog().Value)
            {
                Board board = null;
                try
                {
                    board = Serializer.ImportFromTxt(dialog.FileName);
                    Chess.Reset(board);
                    PlayerModel1.Reset();
                    PlayerModel2.Reset();
                    SetTurnedPlayer();
                    Message.StartBox(Level.Info, "Your board has successfully been imported and is ready to use", "Export Successful");
                }
                catch (Exception e)
                {
                    Message.StartBox(Level.Error, e.Message, "Import Error");
                    return;
                }
            }
        }

        public void ImportXmlAction(object sender)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Choose the XML file to be imported";
            dialog.Filter = "XML|*.xml|All|*.*";
            if (dialog.ShowDialog().Value)
            {
                try
                {
                    Board board = Serializer.FromXml<Board>(dialog.FileName);
                    board.Squares = board.Squares.Skip(64).ToObservableCollection();
                    Chess.Reset(board);
                    PlayerModel1.Reset();
                    PlayerModel2.Reset();
                    SetTurnedPlayer();
                    Message.StartBox(Level.Info, "Your board has successfully been imported and is ready to use", "Export Successful");
                }
                catch (Exception e)
                {
                    Message.StartBox(Level.Error, e.Message, "Import Error");
                    return;
                }
            }
        }

        public void ExportXmlAction(object sender)
        {
            var dialog = new SaveFileDialog();
            dialog.Title = "Choose the saving location of your board";
            dialog.Filter = "XML|*.xml|All|*.*";
            dialog.DefaultExt = ".xml";

            if (dialog.ShowDialog().Value)
            {
                try
                {
                    Chess.Board.ToXml(dialog.FileName);
                    Message.StartBox(Level.Info, "Your board has successfully been exported to: " + dialog.FileName, "Export Successful");
                }
                catch (Exception e)
                {
                    Message.StartBox(Level.Error, e.Message, "Export Error");
                    return;
                }
            }
        }
        #endregion

        #region Private Methods
        private void SetTurnedPlayer()
        {
            if (PlayerModel1.Player.Color == Color.White)
                PlayerModel1.Player.IsMyTurn = true;
            else
                PlayerModel2.Player.IsMyTurn = true;
        }

        private void ClearSelections()
        {
            foreach (var s in Chess.Board.Squares)
            {
                s.IsSelected = false;
                s.IsPossibileSquare = false;
            }

            selectedSquare = null;
        }

        private void ChangeTurns()
        {
            if (PlayerModel1.Player.IsMyTurn)
            {
                PlayerModel1.StopTimer();
                PlayerModel2.StartTimer();
                PlayerModel1.Player.IsMyTurn = false;
                PlayerModel2.Player.IsMyTurn = true;
            }
            else
            {
                PlayerModel2.StopTimer();
                PlayerModel1.StartTimer();
                PlayerModel2.Player.IsMyTurn = false;
                PlayerModel1.Player.IsMyTurn = true;
            }
        }
        #endregion
    }
}
