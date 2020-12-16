using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
//using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace Assignment5
{
    /// <summary>
    /// Ryan Cranston
    /// CSCD 371 Aninimation with WPF assignement
    /// This softwear is simuler to PONG but is only one sided. the frist time you hit the ball it breaks the paddel then next hit the ball gets smaller after that the ball gets even smaller then on the 5th hit the ball gets faster
    /// when the game is running the start button is disabled until pasued. restart can happen at any time. scores are saved on ram till exiting the game when it writes it to the file Leader.scores
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer gameTimer = new DispatcherTimer();
        private static Random r = new Random();
        //private StreamWriter w = File.AppendText(@"Leader.Score");
        private int[]Highscore = new int[5];
        private string line;

        private int hits = 0;
        private int count = 0;
        private int bSpeed = 2;
        private Boolean PaddleChange =false;
        private Boolean BallChange = false;

        private double dx = 1;
        private double dy = 1;
        private double vertDirection = -1;
        private double horizDirection =-1;

        private double gameBallTop = 0;
        private double gameBallLeft = 0;

        private double gamePaddleTop = 0;
        private double gamePaddleLeft = 0;
        private double gamePaddleDy = 2;

        public MainWindow()
        {
            InitializeComponent();

            if (File.Exists(@"Leader.Score"))
            {
                StreamReader R = new StreamReader(@"Leader.Score");
                
                while ((line = R.ReadLine()) != null&& count<5)
                {
                    Highscore[count] = (Int32.Parse(line));
                    count++;
                }
                R.Close();
            }
            while (count < 5) 
            {
                Highscore[count] = 0;
                count++;
            }
            


            gameTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            gameTimer.Tick += GameTimer_Tick;

            gameBallTop = Canvas.GetTop(Ball);
            gameBallLeft = Canvas.GetLeft(Ball);

            gamePaddleTop = Canvas.GetTop(Paddle);
            gamePaddleLeft = Canvas.GetLeft(Paddle);

            randomDir();
            randomLoc();
            gameTimer.IsEnabled = true;
        }

        public void randomDir()
        {
            int xDir= r.Next(-1, 1);
            int yDir= r.Next(-1, 1);
            if(0!=xDir)
                vertDirection = xDir;
            if (0 != yDir)
                horizDirection = yDir;
        }

        public void randomLoc()
        {
            int xLoc = r.Next(0, 255);
            int yLoc = r.Next(300, 600);
            gameBallTop = xLoc;
            gameBallLeft = yLoc;
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            //
            //double xComponent = Ball;
            //double yComponent = 0;
            if (gameBallTop <= 0 || gameBallTop >= (myGameCanvas.Height - Ball.Height))
            {
                vertDirection *= -1;
            }
            if (gameBallLeft <= 0 || gameBallLeft >= myGameCanvas.Width - Ball.Width)
            {
                horizDirection *= -1;
            }
            if (gamePaddleLeft + Paddle.Width == gameBallLeft
                && gameBallTop + Ball.Height / 2 >= gamePaddleTop && gameBallTop + Ball.Height / 2 <= gamePaddleTop + Paddle.Height)
            {
                hits++;
                horizDirection = 1;

            }
            //Difficulty Clause
                //paddle
                if(hits==1 && PaddleChange == false)
                {
                    Status.Content = "Looks like your paddle broke!";
                    PaddleChange = true;
                    Paddle.Height = 75;
                }
                //ball
                if (hits == 2 && BallChange == false)
                {
                    Status.Content = "Do you need glasses or is the ball getting smaller?";
                    Ball.Height = 20;
                    Ball.Width = Ball.Height;
                }
                if (hits == 3 && BallChange == false)
                {
                    Status.Content = "oof, ... good luck";
                    Ball.Height = 10;
                    Ball.Width = Ball.Height;
                }
            if (hits == 5 && BallChange == false)
            {
                Status.Content = "ouch";
                bSpeed = 3;
                BallChange = true;
            }

            //ending clause
            if (gameBallLeft <= 0)
            {
                Status.Content = "get gud";
                gameTimer.IsEnabled = false;
                Paddle.Visibility = Visibility.Hidden;
                MessageBox.Show($"You Lose\n your score is:{hits}");
                for(int i =0;i<5;i++)
                {
                    if (Highscore[i]<hits)
                    {
                        Highscore[i] = hits;
                        break;
                    }
                }
            }

            gameBallLeft += dx * horizDirection* bSpeed;
            gameBallTop += dy * vertDirection* bSpeed;

            Canvas.SetTop(Ball, gameBallTop);
            Canvas.SetLeft(Ball, gameBallLeft);
            Score.Content = hits;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                if (gamePaddleTop - gamePaddleDy+2 >= 0)
                    gamePaddleTop -= gamePaddleDy + 2;
            }
            else if (e.Key == Key.Down)
            {
                if (gamePaddleTop + gamePaddleDy + Paddle.Height+2 <= myGameCanvas.Height)
                    gamePaddleTop += gamePaddleDy+2;
            }
            Canvas.SetTop(Paddle, gamePaddleTop);
        }

        private void mnuReset(object sender, RoutedEventArgs e)
        {
            
            Status.Content = "Lets try this again";
            Paddle.Visibility = Visibility.Visible;
            randomLoc();
            randomDir();
            bSpeed = 2;
            hits = 0;
            Paddle.Height = 100;
            PaddleChange = false;
            BallChange = false;
            MessageBox.Show("Ready to Begin?");
            gameTimer.IsEnabled = true;
        }

        private void mnuPause(object sender, RoutedEventArgs e)
        {
            MnuPause.IsEnabled = false;
            Status.Content = "Paused";
            gameTimer.IsEnabled = false;
            Paddle.Visibility = Visibility.Hidden;
            MnuStart.IsEnabled = true;
        }
        private void mnuStart(object sender, RoutedEventArgs e)
        {
            MnuStart.IsEnabled = false;
            Status.Content = "Lets Play";
            gameTimer.IsEnabled = true;
            Paddle.Visibility = Visibility.Visible;
            MnuPause.IsEnabled = true;
        }
        private void mnuExit(object sender, RoutedEventArgs e)
        {
            const string message = "Are you sure you'd like to leave the game?";
            const string caption = "But Wait...";
            var result = MessageBox.Show(message, caption,
                                         MessageBoxButton.YesNo);
            if ((int)result != 7)
            {
                StreamWriter w = new StreamWriter(@"Leader.Score");
                for (int i = 0; i < 5; i++)
                {
                    w.Write(Highscore[i]+"\n");
                    w.Flush();
                }
                w.Close();
                Environment.Exit(0);
            }
        }
        private void mnuLeader(object sender, RoutedEventArgs e)
        {
            Status.Content = "Whats your rank?";
            MessageBox.Show($"Current high scores:\n" +
                $"{Highscore[0]}\n" +
                $"{Highscore[1]}\n" +
                $"{Highscore[2]}\n" +
                $"{Highscore[3]}\n" +
                $"{Highscore[4]}\n");

        }
        private void mnuHelpAbout(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Lonely Persons Pong\n, A fun slow to fast pace game that get more difficult as you go along, \nBased on the .Net Core and developed using WPF \nConstructed by Ryan Cranston");
        }
    }
}
