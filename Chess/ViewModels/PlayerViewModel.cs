using Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using System.Windows.Threading;

namespace Chess.ViewModels
{
    public class PlayerViewModel : PropertyChangedBase
    {
        private DispatcherTimer timer;
        private string timeRemaining;
        private string date;
        private TimeSpan startingTime;
        private bool piecesAreSelectable;
        private bool showPossibleMoves;

        public PlayerViewModel(Board board, Color color)
        {
            Player = new Player(board, color);

            ReviveCommand = new ActionCommand(ReviveAction);

            startingTime = TimeSpan.FromMinutes(5);
            TimeRemaining = startingTime.ToString(@"mm\:ss");
            Date = DateTime.Now.ToShortDateString();
            InititializeTimer();
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


        private void InititializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;            
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
            PiecesAreSelectable = false;
            UpgradeSelected?.Invoke(piece, new EventArgs());
        }

        public event EventHandler TimeIsOver;

        public event EventHandler UpgradeSelected;

        public bool PiecesAreSelectable
        {
            get { return piecesAreSelectable; }
            set { RaisePropertyChanged(ref piecesAreSelectable, value); }
        }

        public Piece TargetPiece { get; set; }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (startingTime == TimeSpan.Zero) TimeIsOver?.Invoke(this, new EventArgs());
            startingTime = startingTime.Add(TimeSpan.FromSeconds(-1));
            TimeRemaining = startingTime.ToString(@"mm\:ss");
            Date = DateTime.Now.ToShortDateString();
        }
    }
}
