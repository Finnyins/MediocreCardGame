// Finn O'Brien
// Project 24 (Main window)
// 12/7/2021


using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
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

namespace Project24
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        byte Gamemode = 0;

        List<MDL_Stats> stats = StatsManager.Load();

        SCR_Deck PlrDeck = new SCR_Deck();
        SCR_Deck CPUDeck = new SCR_Deck();

        bool turn = false;
        bool defend = false;

        List<string> CPUHand = new List<string>();
        List<string> PlrHand = new List<string>();

        byte PlrPoints = 0;
        byte CPUPoints = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void StartGame()
        {
            PlrPoints = 0;
            CPUPoints = 0;
            LBL_PlrPnt.Content = PlrPoints;
            LBL_CPUPnts.Content = CPUPoints;
            DealCards();
            PlayerTurn();
        }
        private void DealCards()
        {
            for (int i = 0; i < 5; i++)
            {
                PlrHand.Add(PlrDeck.Draw());
                CPUHand.Add(CPUDeck.Draw());
            }
            BTN_CardI.Content = PlrHand[0];
            BTN_CardII.Content = PlrHand[1];
            BTN_CardIII.Content = PlrHand[2];
            BTN_CardIV.Content = PlrHand[3];
            BTN_CardV.Content = PlrHand[4];
            LBL_CPUDeck.Content = CPUDeck.cards.Count();
            LBL_PlrDeck.Content = PlrDeck.cards.Count();
            RCT_EcardI.Visibility = Visibility.Visible;
            RCT_EcardII.Visibility = Visibility.Visible;
            RCT_EcardIII.Visibility = Visibility.Visible;
            RCT_EcardIV.Visibility = Visibility.Visible;
            RCT_EcardV.Visibility = Visibility.Visible;
            BTN_CardI.Visibility = Visibility.Visible;
            BTN_CardII.Visibility = Visibility.Visible;
            BTN_CardIII.Visibility = Visibility.Visible;
            BTN_CardIV.Visibility = Visibility.Visible;
            BTN_CardV.Visibility = Visibility.Visible;
        }
        
        private void EnableCards()
        {
            switch (PlrHand.Count)
            {
                case 5:
                    BTN_CardI.IsEnabled = true;
                    BTN_CardII.IsEnabled = true;
                    BTN_CardIII.IsEnabled = true;
                    BTN_CardIV.IsEnabled = true;
                    BTN_CardV.IsEnabled = true;
                    break;

                case 4:
                    BTN_CardI.IsEnabled = true;
                    BTN_CardII.IsEnabled = true;
                    BTN_CardIII.IsEnabled = true;
                    BTN_CardIV.IsEnabled = true;
                    break;

                case 3:
                    BTN_CardI.IsEnabled = true;
                    BTN_CardII.IsEnabled = true;
                    BTN_CardIII.IsEnabled = true;
                    break;

                case 2:
                    BTN_CardI.IsEnabled = true;
                    BTN_CardII.IsEnabled = true;
                    break;

                case 1:
                    BTN_CardI.IsEnabled = true;
                    break;
            }
            BTN_CardI.IsEnabled = true;
            BTN_CardII.IsEnabled = true;
            BTN_CardIII.IsEnabled = true;
            BTN_CardIV.IsEnabled = true;
            BTN_CardV.IsEnabled = true;
        }
        private void DisableCards()
        {
            BTN_CardI.IsEnabled = false;
            BTN_CardII.IsEnabled = false;
            BTN_CardIII.IsEnabled = false;
            BTN_CardIV.IsEnabled = false;
            BTN_CardV.IsEnabled = false;
        }
        private void PlayerTurn()
        {
            Trace.WriteLine("Player Turn");
            LBL_Who.Content = "Your";
            turn = true;
            EnableCards();
            if (PlrHand.Count == 0)
            {
                turn = false;
                PrepEndTurn(true, 0);
            }
        }

        async Task PlayerDefend()
        {
            Trace.WriteLine("Player Defend");
            LBL_Who.Content = "Your";
            LBL_Turn.Content = "Turn";
            defend = true;
            EnableCards();
            BTN_Pass.IsEnabled = true;
            BTN_Pass.Visibility = Visibility.Visible;
            if (PlrHand.Count == 0)
            {
                turn = false;
                if (LBL_Playcard.Content == "Wild")
                {
                    PrepEndTurn(false, 0);
                }
                else
                {
                    PrepEndTurn(false, Convert.ToByte(LBL_Playcard.Content));
                }
            }
        }

        private void CPUTurn()
        {
            Trace.WriteLine("CPU Turn");
            ReadCPUHand();
            byte highestcard = 0;
            byte cards = 0;
            bool hasonlywilds = true;
            foreach (string card in CPUHand)
            {
                cards++;
                if (card != "Wild")
                {
                    hasonlywilds = false;
                    if (Convert.ToByte(card) > highestcard)
                    {
                        highestcard = Convert.ToByte(card);
                    }
                }
            }
            if (cards == 0)
            {
                LBL_Playcard.Visibility = Visibility.Visible;
            }
            if (hasonlywilds && cards != 0)
            {
                LBL_Playcard.Visibility = Visibility.Visible;
                LBL_Playcard.Foreground = Brushes.Red;
                LBL_Playcard.Content = "Wild";
                CPUHand.Remove("Wild");
            }
            else
            {
                LBL_Playcard.Visibility = Visibility.Visible;
                LBL_Playcard.Foreground = Brushes.Red;
                LBL_Playcard.Content = Convert.ToString(highestcard);
                CPUHand.Remove(Convert.ToString(highestcard));
            }
            if (RCT_EcardV.IsVisible)
            {
                RCT_EcardV.Visibility = Visibility.Hidden;
            }
            else if (RCT_EcardIV.IsVisible)
            {
                RCT_EcardIV.Visibility = Visibility.Hidden;
            }
            else if (RCT_EcardIII.IsVisible)
            {
                RCT_EcardIII.Visibility = Visibility.Hidden;
            }
            else if (RCT_EcardII.IsVisible)
            {
                RCT_EcardII.Visibility = Visibility.Hidden;
            }
            else if (RCT_EcardI.IsVisible)
            {
                RCT_EcardI.Visibility = Visibility.Hidden;
            }
            if (cards != 0)
            {
                LBL_Playcard.Visibility = Visibility.Visible;
            }
            if (LBL_Playcard.Content == "Wild")
            {
                PrepEndTurn(false, 0);
            }
            else if (cards != 0)
            {
                PlayerDefend();
            }
            else
            {
                PrepEndTurn(false, 0);
            }
        }

        async Task PrepCPUTurn()
        {
            await Task.Delay(2500);
            CPUTurn();
        }

        private void ReadCPUHand()
        {
            string cards = "";
            foreach (string card in CPUHand)
            {
                cards += $"{card}, ";
            }
            Trace.WriteLine(cards);
        }
        private void GRD_Main_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // This is nothing but the remnants of a failed scaling setup
        }

        async Task EndGame()
        {
            await Task.Delay(2500);
            CNV_Victory.IsEnabled = false;
            CNV_Victory.Visibility = Visibility.Hidden;
            GRD_Play.IsEnabled = false;
            GRD_Play.Visibility = Visibility.Hidden;
            CNV_Menu.IsEnabled = true;
            CNV_Menu.Visibility = Visibility.Visible;
        }

        async Task EndTurn(bool plrscore, byte points)
        {
            Trace.WriteLine("End Turn");
            await Task.Delay(2500);
            LBL_Playcard.Visibility = Visibility.Hidden;
            BTN_Pass.IsEnabled = false;
            BTN_Pass.Visibility = Visibility.Hidden;
            if (plrscore)
            {
                PlrPoints += points;
                LBL_PlrPnt.Content = PlrPoints;
                LBL_Who.Content = "CPU's";
                LBL_Turn.Content = "Turn";
            }
            else
            {
                CPUPoints += points;
                LBL_CPUPnts.Content = CPUPoints;
                LBL_Who.Content = "Your";
                LBL_Turn.Content = "Turn";
            }
            string Plrdraw = null;
            if (PlrHand.Count < 5)
            {
                Plrdraw = PlrDeck.Draw();
            }
            if (Plrdraw != null)
            {
                PlrHand.Add(Plrdraw);
                BTN_CardV.Content = PlrHand[4];
                BTN_CardV.Visibility = Visibility.Visible;
            }
            string CPUdraw = null;
            if (CPUHand.Count < 5)
            {
                CPUdraw = CPUDeck.Draw();
            }
            if (CPUdraw != null)
            {
                CPUHand.Add(CPUdraw);
                RCT_EcardV.Visibility = Visibility.Visible;
                RCT_EcardIV.Visibility = Visibility.Visible;
                RCT_EcardIII.Visibility = Visibility.Visible;
            }
            LBL_CPUDeck.Content = CPUDeck.cards.Count();
            LBL_PlrDeck.Content = PlrDeck.cards.Count();
            if (PlrHand.Count == 0 && CPUHand.Count == 0)
            {
                if (PlrPoints > CPUPoints)
                {
                    LBL_Victor.Content = "Player";
                    stats[Gamemode].Wins += 1;
                    
                }
                else if (PlrPoints < CPUPoints)
                {
                    LBL_Victor.Content = "CPU";
                    stats[Gamemode].Losses += 1;
                }
                else
                {
                    LBL_Victor.Content = "Draw. Nobody";
                    stats[Gamemode].Ties += 1;
                }
                CNV_Victory.Visibility = Visibility.Visible;
                StatsManager.Update(stats[Gamemode]);
                await EndGame();
                return;
            }
            ReadCPUHand();
            if (plrscore)
            {
                await PrepCPUTurn();
            }
            else
            {
                PlayerTurn();
            }
        }

        private async void PrepEndTurn(bool plrscore, byte points)
        {
            await EndTurn(plrscore, points);
        }



        private void CPUDefend()
        {
            Trace.WriteLine("CPU Defend");
            if (LBL_Playcard.Content == "Wild")
            {
                PrepEndTurn(true, 0);
            }
            else
            {
                sbyte remainder = 0;
                bool haswild = false;
                bool hasequal = false;
                bool hasgreater = false;
                bool hasnumbercard = false;
                bool playedcard = true;
                byte highestcard = 0;
                byte lowestcard = 10;
                byte lowestgreater = 10;
                byte highestless = 0;
                byte cards = 0;
                foreach (string card in CPUHand)
                {
                    cards++;
                    if (card != "Wild")
                    {
                        hasnumbercard = true;
                        if (Convert.ToByte(card) > highestcard)
                        {
                            highestcard = Convert.ToByte(card);
                        }
                        if (Convert.ToByte(card) < lowestcard)
                        {
                            lowestcard = Convert.ToByte(card);
                        }
                        if (Convert.ToByte(card) < Convert.ToByte(LBL_Playcard.Content) && Convert.ToByte(card) > highestless)
                        {
                            highestless = Convert.ToByte(card);
                        }
                    }
                    if (card == "Wild")
                    {
                        haswild = true;
                    }
                    else if (Convert.ToByte(card) == Convert.ToByte(LBL_Playcard.Content))
                    {
                        hasequal = true;
                    }
                    else if (Convert.ToByte(card) > Convert.ToByte(LBL_Playcard.Content))
                    {
                        hasgreater = true;
                        if (Convert.ToByte(card) < lowestgreater)
                        {
                            lowestgreater = Convert.ToByte(card);
                        }
                    }
                }
                if (cards == 0)
                {
                    LBL_Playcard.Visibility = Visibility.Hidden;
                    remainder = Convert.ToSByte(LBL_Playcard.Content);
                }
                else if (haswild && Convert.ToByte(LBL_Playcard.Content) > 5 && CPUHand.Count != 0)
                {
                    LBL_Playcard.Foreground = Brushes.Red;
                    LBL_Playcard.Content = "Wild";
                    CPUHand.Remove("Wild");
                    remainder = 0;
                }
                else if (hasequal)
                {
                    LBL_Playcard.Foreground = Brushes.Red;
                    CPUHand.Remove(Convert.ToString(LBL_Playcard.Content));
                    remainder = 0;
                }
                else if (hasgreater && Convert.ToByte(LBL_Playcard.Content) <= 5 && lowestcard > Convert.ToByte(Convert.ToByte(LBL_Playcard.Content) + 2))
                {
                    playedcard = false;
                    remainder = Convert.ToSByte(LBL_Playcard.Content);
                }
                else if (hasgreater && lowestcard > Convert.ToByte(LBL_Playcard.Content))
                {
                    LBL_Playcard.Foreground = Brushes.Red;
                    LBL_Playcard.Content = Convert.ToString(lowestcard);
                    CPUHand.Remove(Convert.ToString(lowestcard));
                    remainder = 0;
                }
                else if (hasgreater && highestless < Convert.ToByte(Math.Max(0, Convert.ToByte(LBL_Playcard.Content) - 2)))
                {
                    LBL_Playcard.Foreground = Brushes.Red;
                    LBL_Playcard.Content = Convert.ToString(lowestgreater);
                    CPUHand.Remove(Convert.ToString(lowestgreater));
                    remainder = 0;
                }
                else if (hasgreater && highestless >= Convert.ToByte(Convert.ToByte(LBL_Playcard.Content) - 1) && lowestgreater > Convert.ToByte(Convert.ToByte(LBL_Playcard.Content) + 2))
                {
                    remainder = Convert.ToSByte(Convert.ToByte(LBL_Playcard.Content) - highestless);
                    if (remainder < 0)
                    {
                        remainder = 0;
                    }
                    LBL_Playcard.Foreground = Brushes.Red;
                    LBL_Playcard.Content = Convert.ToString(highestless);
                    CPUHand.Remove(Convert.ToString(highestless));
                }

                else if (hasgreater && highestless < Convert.ToByte(Convert.ToByte(LBL_Playcard.Content) - 2))
                {
                    remainder = 0;
                    LBL_Playcard.Foreground = Brushes.Red;
                    LBL_Playcard.Content = Convert.ToString(lowestgreater);
                    CPUHand.Remove(Convert.ToString(lowestgreater));
                }
                else if (highestcard == 0 && CPUHand.Count > 0)
                {
                    remainder = 0;
                    LBL_Playcard.Foreground = Brushes.Red;
                    LBL_Playcard.Content = "Wild";
                    CPUHand.Remove("Wild");
                }
                else if (hasgreater)
                {
                    remainder = 0;
                    LBL_Playcard.Foreground = Brushes.Red;
                    LBL_Playcard.Content = Convert.ToString(lowestgreater);
                    CPUHand.Remove(Convert.ToString(lowestgreater));
                }
                else if (hasnumbercard && highestcard != 0 && CPUHand.Count > 0)
                {
                    remainder = Convert.ToSByte(Convert.ToByte(LBL_Playcard.Content) - highestcard);
                    if (remainder < 0)
                    {
                        remainder = 0;
                    }
                    LBL_Playcard.Foreground = Brushes.Red;
                    LBL_Playcard.Content = Convert.ToString(highestcard);
                    CPUHand.Remove(Convert.ToString(highestcard));
                }
                else
                {
                    playedcard = false;
                    remainder = Convert.ToSByte(LBL_Playcard.Content);
                }
                if (playedcard)
                {
                    if (RCT_EcardV.IsVisible)
                    {
                        RCT_EcardV.Visibility = Visibility.Hidden;
                    }
                    else if (RCT_EcardIV.IsVisible)
                    {
                        RCT_EcardIV.Visibility = Visibility.Hidden;
                    }
                    else if (RCT_EcardIII.IsVisible)
                    {
                        RCT_EcardIII.Visibility = Visibility.Hidden;
                    }
                    else if (RCT_EcardII.IsVisible)
                    {
                        RCT_EcardII.Visibility = Visibility.Hidden;
                    }
                    else if (RCT_EcardI.IsVisible)
                    {
                        RCT_EcardI.Visibility = Visibility.Hidden;
                    }
                }
                PrepEndTurn(true, Convert.ToByte(remainder));
            }
        }

        async Task PrepCPUDefend()
        {
            await Task.Delay(2500);
            CPUDefend();
        }

        private async void CardClick(object sender, RoutedEventArgs e)
        {
            string value = "";
            string name = (sender as Button).Name.ToString();
            if (turn)
            {
                LBL_Playcard.Visibility = Visibility.Visible;
                switch (name)
                {
                    case "BTN_CardI":
                        value = PlrHand[0];
                        LBL_Playcard.Foreground = Brushes.Green;
                        LBL_Playcard.Content = value;
                        PlrHand.RemoveAt(0);
                        break;

                    case "BTN_CardII":
                        value = PlrHand[1];
                        LBL_Playcard.Foreground = Brushes.Green;
                        LBL_Playcard.Content = value;
                        PlrHand.RemoveAt(1);
                        break;

                    case "BTN_CardIII":
                        value = PlrHand[2];
                        LBL_Playcard.Foreground = Brushes.Green;
                        LBL_Playcard.Content = value;
                        PlrHand.RemoveAt(2);
                        break;

                    case "BTN_CardIV":
                        value = PlrHand[3];
                        LBL_Playcard.Foreground = Brushes.Green;
                        LBL_Playcard.Content = value;
                        PlrHand.RemoveAt(3);
                        break;

                    case "BTN_CardV":
                        value = PlrHand[4];
                        LBL_Playcard.Foreground = Brushes.Green;
                        LBL_Playcard.Content = value;
                        PlrHand.RemoveAt(4);
                        break;
                }
                if (BTN_CardV.IsVisible)
                {
                    BTN_CardV.IsEnabled = false;
                    BTN_CardV.Visibility = Visibility.Hidden;
                    BTN_CardI.Content = PlrHand[0];
                    BTN_CardII.Content = PlrHand[1];
                    BTN_CardIII.Content = PlrHand[2];
                    BTN_CardIV.Content = PlrHand[3];
                }
                else if (BTN_CardIV.IsVisible)
                {
                    BTN_CardIV.IsEnabled = false;
                    BTN_CardIV.Visibility = Visibility.Hidden;
                    BTN_CardI.Content = PlrHand[0];
                    BTN_CardII.Content = PlrHand[1];
                    BTN_CardIII.Content = PlrHand[2];
                }
                else if (BTN_CardIII.IsVisible)
                {
                    BTN_CardIII.IsEnabled = false;
                    BTN_CardIII.Visibility = Visibility.Hidden;
                    BTN_CardI.Content = PlrHand[0];
                    BTN_CardII.Content = PlrHand[1];
                }
                else if (BTN_CardII.IsVisible)
                {
                    BTN_CardII.IsEnabled = false;
                    BTN_CardII.Visibility = Visibility.Hidden;
                    BTN_CardI.Content = PlrHand[0];
                }
                else if (BTN_CardI.IsVisible)
                {
                    BTN_CardI.IsEnabled = false;
                    BTN_CardI.Visibility = Visibility.Hidden;
                }
                turn = false;
                LBL_Who.Content = "CPU's";
                LBL_Turn.Content = "Turn";
                DisableCards();
                await PrepCPUDefend();
            }
            else if (defend)
            {
                BTN_Pass.IsEnabled = false;
                BTN_Pass.Visibility = Visibility.Hidden;
                sbyte remainder = 0;
                LBL_Playcard.Visibility = Visibility.Visible;
                byte played = Convert.ToByte(LBL_Playcard.Content);
                switch (name)
                {
                    case "BTN_CardI":
                        value = PlrHand[0];
                        LBL_Playcard.Foreground = Brushes.Green;
                        LBL_Playcard.Content = value;
                        PlrHand.RemoveAt(0);
                        break;

                    case "BTN_CardII":
                        value = PlrHand[1];
                        LBL_Playcard.Foreground = Brushes.Green;
                        LBL_Playcard.Content = value;
                        PlrHand.RemoveAt(1);
                        break;

                    case "BTN_CardIII":
                        value = PlrHand[2];
                        LBL_Playcard.Foreground = Brushes.Green;
                        LBL_Playcard.Content = value;
                        PlrHand.RemoveAt(2);
                        break;

                    case "BTN_CardIV":
                        value = PlrHand[3];
                        LBL_Playcard.Foreground = Brushes.Green;
                        LBL_Playcard.Content = value;
                        PlrHand.RemoveAt(3);
                        break;

                    case "BTN_CardV":
                        value = PlrHand[4];
                        LBL_Playcard.Foreground = Brushes.Green;
                        LBL_Playcard.Content = value;
                        PlrHand.RemoveAt(4);
                        break;
                }
                if (BTN_CardV.IsVisible)
                {
                    BTN_CardV.IsEnabled = false;
                    BTN_CardV.Visibility = Visibility.Hidden;
                    BTN_CardI.Content = PlrHand[0];
                    BTN_CardII.Content = PlrHand[1];
                    BTN_CardIII.Content = PlrHand[2];
                    BTN_CardIV.Content = PlrHand[3];
                }
                else if (BTN_CardIV.IsVisible)
                {
                    BTN_CardIV.IsEnabled = false;
                    BTN_CardIV.Visibility = Visibility.Hidden;
                    BTN_CardI.Content = PlrHand[0];
                    BTN_CardII.Content = PlrHand[1];
                    BTN_CardIII.Content = PlrHand[2];
                }
                else if (BTN_CardIII.IsVisible)
                {
                    BTN_CardIII.IsEnabled = false;
                    BTN_CardIII.Visibility = Visibility.Hidden;
                    BTN_CardI.Content = PlrHand[0];
                    BTN_CardII.Content = PlrHand[1];
                }
                else if (BTN_CardII.IsVisible)
                {
                    BTN_CardII.IsEnabled = false;
                    BTN_CardII.Visibility = Visibility.Hidden;
                    BTN_CardI.Content = PlrHand[0];
                }
                else if (BTN_CardI.IsVisible)
                {
                    BTN_CardI.IsEnabled = false;
                    BTN_CardI.Visibility = Visibility.Hidden;
                }
                if (value == "Wild")
                {
                    remainder = 0;
                }
                else
                {
                    remainder = Convert.ToSByte(played - Convert.ToByte(value));
                    if (remainder < 0)
                    {
                        remainder = 0;
                    }
                }
                defend = false;
                DisableCards();
                PrepEndTurn(false, Convert.ToByte(remainder));
            }
        }

        private void BTN_Pass_Click(object sender, RoutedEventArgs e)
        {
            if (defend)
            {
                BTN_Pass.IsEnabled = false;
                BTN_Pass.Visibility = Visibility.Hidden;
                DisableCards();
                if (LBL_Playcard.Content == "Wild")
                {
                    defend = false;
                    EndTurn(false, 0);
                }
                else
                {
                    defend = false;
                    EndTurn(false, Convert.ToByte(LBL_Playcard.Content));
                }
            }
        }

        private void BTN_PlayGame_Click(object sender, RoutedEventArgs e)
        {
            CNV_Menu.IsEnabled = false;
            CNV_Menu.Visibility = Visibility.Hidden;
            CNV_GameSelect.IsEnabled = true;
            CNV_GameSelect.Visibility = Visibility.Visible;
        }

        private void BTN_Stats_Click(object sender, RoutedEventArgs e)
        {
            CNV_Menu.IsEnabled = false;
            CNV_Menu.Visibility = Visibility.Hidden;
            CNV_Stats.IsEnabled = true;
            CNV_Stats.Visibility = Visibility.Visible;
            stats = StatsManager.Load();
            LBL_ClassWin.Content = stats[0].Wins;
            LBL_ClassLoss.Content = stats[0].Losses;
            LBL_ClassTies.Content = stats[0].Ties;
            LBL_ShareWin.Content = stats[1].Wins;
            LBL_ShareLoss.Content = stats[1].Losses;
            LBL_ShareTie.Content = stats[1].Ties;
        }

        private void BTN_Quit_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult quit = MessageBox.Show("Quit the game?\n\n\nHonestly I don't blame you", "Quit Game", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            if (quit == MessageBoxResult.Yes)
            {
                Close();
            }
        }

        private void BTN_Back_Click(object sender, RoutedEventArgs e)
        {
            CNV_GameSelect.IsEnabled = false;
            CNV_GameSelect.Visibility = Visibility.Hidden;
            CNV_Stats.IsEnabled = false;
            CNV_Stats.Visibility = Visibility.Hidden;
            CNV_Menu.IsEnabled = true;
            CNV_Menu.Visibility = Visibility.Visible;
        }

        private void BTN_Classic_Click(object sender, RoutedEventArgs e)
        {
            CNV_GameSelect.IsEnabled = false;
            CNV_GameSelect.Visibility = Visibility.Hidden;
            GRD_Play.IsEnabled = true;
            GRD_Play.Visibility = Visibility.Visible;
            Gamemode = 0;
            PlrDeck = new SCR_Deck();
            CPUDeck = new SCR_Deck();
            StartGame();
        }

        private void BTN_SharedDeck_Click(object sender, RoutedEventArgs e)
        {
            CNV_GameSelect.IsEnabled = false;
            CNV_GameSelect.Visibility = Visibility.Hidden;
            GRD_Play.IsEnabled = true;
            GRD_Play.Visibility = Visibility.Visible;
            Gamemode = 1;
            PlrDeck = new SCR_Deck();
            CPUDeck = PlrDeck;
            StartGame();
        }
    }
}
