using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Input;
using Library;
using Microsoft.Win32;

namespace Chess.ViewModels
{
    public class ApplicationViewModel : PropertyChangedBase
    {
        #region Private Fields
        private GameOverViewModel gameOverModel;
        private ApplicationSettings settings;        
        private Square selectedSquare;
        private string settingsPath = @"..\..\..\..\Library\Xml\Settings.xml";
        #endregion

        #region Constructors
        public ApplicationViewModel()
        {
            settings = Serializer.FromXml<ApplicationSettings>(settingsPath);

            Chess = new Game(settings);

            Chess.Board.OnInitiatePawnPromotion += Board_OnInitiatePawnPromotion;

            PlayerModel1 = new PlayerViewModel(Chess.Board, Color.White, settings.PlayerTimeInMinutes);
            PlayerModel2 = new PlayerViewModel(Chess.Board, Color.Black, settings.PlayerTimeInMinutes);
            PlayerModel1.Player.UserName = "Player1";           
            PlayerModel2.Player.UserName = "Player2";

            PlayerModel1.Player.OnGameOver += Player_OnGameOver;
            PlayerModel2.Player.OnGameOver += Player_OnGameOver;
            PlayerModel1.TimeIsOver += Player_OnGameOver;
            PlayerModel2.TimeIsOver += Player_OnGameOver;

            PlayerModel1.Player.OnPieceRemoved += Player_OnPieceRemoved;     
            PlayerModel2.Player.OnPieceRemoved += Player_OnPieceRemoved;            

            PlayerModel1.OnPromotionSelected += PlayerModel_OnPromotionSelected; 
            PlayerModel2.OnPromotionSelected += PlayerModel_OnPromotionSelected;

            ImportTxtCommand = new ActionCommand(ImportTxtAction);

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

        public ICommand SquareCommand { get; }
        #endregion

        #region Events and Event-Methods
        private void Board_OnInitiatePawnPromotion(object sender, EventArgs e)
        {
            PlayerAtTurn.ReceivingPiece = sender as Piece;
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

        private void Player_OnPieceRemoved(object sender, EventArgs e)
        {
            PlayerAtWait.Player.LostPieces.Add(sender as Piece);
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

            if (selectedSquare != null && !square.Point.Equals(selectedSquare.Point))
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

                if (PlayerAtTurn.Player.ShowPossibleMoves)
                    PlayerAtTurn.Player.PossibleMoves(selectedSquare.Piece);
            }
        }

        public void ImportTxtAction(object sender)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Choose the txt to be converted";
            dialog.Filter = "Text|*.txt|All|*.*";

            if (dialog.ShowDialog().Value)
            {
                var board = Serializer.ImportFromTxt(dialog.FileName);
                if (board is string)
                    Message.StartBox(Level.Error, board as string, "An error occured while import of the txt at " + dialog.FileName);
                else
                {
                    Chess.Reset(board as Board);
                    PlayerModel1.Reset();
                    PlayerModel2.Reset();
                    SetTurnedPlayer();
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
