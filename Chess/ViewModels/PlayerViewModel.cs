using Library;
using Library.Pieces;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;

namespace Chess.ViewModels
{
    public class PlayerViewModel : PropertyChangedBase
    {
        private TimeSpan startingTime;
        private bool piecesAreSelectable;
        private Piece selectedPromotion;
        private int playerTimeInMinutes;
        private DispatcherTimer timer;
        private string timeRemaining;
        private string date;

        public PlayerViewModel(Board board, Color color, int playerTimeInMinutes)
        {
            this.playerTimeInMinutes = playerTimeInMinutes;

            Player = new Player(board, color);

            Promotables = new ObservableCollection<Piece>();
            Promotables.Add(new Queen() { Color = color });
            Promotables.Add(new Knight() { Color = color });
            Promotables.Add(new Bishop() { Color = color });
            Promotables.Add(new Rook() { Color = color });

            ReviveCommand = new ActionCommand(ReviveAction);

            startingTime = TimeSpan.FromMinutes(playerTimeInMinutes);
            TimeRemaining = startingTime.ToString(@"mm\:ss");
            Date = DateTime.Now.ToShortDateString();
            ResetTimer();
        }

        public string TimeRemaining
        {
            get { return timeRemaining; }
            set { RaisePropertyChanged(ref timeRemaining, value); }
        }

        public string Date
        {
            get { return date; }
            set { RaisePropertyChanged(ref date, value); }
        }

        public Player Player { get; set; }

        public ICommand ReviveCommand { get; set; }

        public ObservableCollection<Piece> Promotables { get; }

        public void ResetTimer()
        {
            timer = new DispatcherTimer();
            startingTime = TimeSpan.FromMinutes(playerTimeInMinutes);
            TimeRemaining = startingTime.ToString(@"mm\:ss");
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            StopTimer();
        }

        public void Reset()
        {
            ResetTimer();
            Player.LostPieces.Clear();
            Player.IsMyTurn = false;
        }

        public void StartTimer()
        {
            timer.Start();
        }

        public void StopTimer()
        {
            timer.Stop();
        }

        public void ReviveAction(object sender)
        {
            var piece = sender as Piece;
            OnPromotionSelected?.Invoke(piece, new EventArgs());
            PiecesAreSelectable = false;
        }

        public event EventHandler TimeIsOver;

        public event EventHandler OnPromotionSelected;

        public bool PiecesAreSelectable
        {
            get { return piecesAreSelectable; }
            set
            {
                RaisePropertyChanged(ref piecesAreSelectable, value);
                if (piecesAreSelectable)
                {
                    StopTimer();
                }
            }
        }

        public Piece ReceivingPiece
        {
            get { return selectedPromotion; }
            set
            {
                RaisePropertyChanged(ref selectedPromotion, value);
                if (selectedPromotion != null)
                {
                    PiecesAreSelectable = true;
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (startingTime == TimeSpan.Zero)
            {
                TimeIsOver?.Invoke(Player, new GameOverEventArgs(GameOver.Time));
            }

            startingTime = startingTime.Add(TimeSpan.FromSeconds(-1));
            TimeRemaining = startingTime.ToString(@"mm\:ss");
            Date = DateTime.Now.ToShortDateString();
        }
    }
}
