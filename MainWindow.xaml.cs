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
        SCR_Deck PlrDeck = new SCR_Deck();
        SCR_Deck CPUDeck = new SCR_Deck();

        bool turn = false;

        List<string> CPUHand = new List<string>();
        List<string> PlrHand = new List<string>();

        byte PlrPoints = 0;
        byte CPUPoints = 0;

        public MainWindow()
        {
            InitializeComponent();
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
            ReadCPUHand();
        }
        
        private void PlayerTurn()
        {
            LBL_Who.Content = "Your";
            turn = true;
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

        async Task EndTurn(bool plrscore, byte points)
        {
            await Task.Delay(2500);
            LBL_Playcard.Visibility = Visibility.Hidden;
            if (plrscore)
            {
                PlrPoints += points;
                LBL_PlrPnt.Content = PlrPoints;
            }
            else
            {
                CPUPoints += points;
                LBL_CPUPnts.Content = CPUPoints;
            }
            CPUHand.Add(CPUDeck.Draw());
            ReadCPUHand();
            RCT_EcardV.Visibility = Visibility.Visible;
            PlrHand.Add(PlrDeck.Draw());
            BTN_CardV.Content = PlrHand[4];
            BTN_CardV.IsEnabled = true;
            BTN_CardV.Visibility = Visibility.Visible;
        }

        private async void PrepEndTurn(bool plrscore, byte points)
        {
            await EndTurn(plrscore, points);
        }



        private void CPUDefend()
        {
            if (LBL_Playcard.Content == "Wild")
            {
                EndTurn(true, 0);
            }
            else
            {
                byte remainder = 0;
                bool haswild = false;
                bool hasequal = false;
                bool hasgreater = false;
                byte highestcard = 0;
                byte lowestcard = 10;
                byte lowestgreater = 10;
                foreach (string card in CPUHand)
                {
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
                    else if (Convert.ToByte(card) > highestcard)
                    {
                        highestcard = Convert.ToByte(card);
                    }
                    else if (Convert.ToByte(card) < lowestcard)
                    {
                        lowestcard = Convert.ToByte(card);
                    }
                }
                if (haswild && Convert.ToByte(LBL_Playcard.Content) > 5)
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
                else if (hasgreater && lowestcard > Convert.ToByte(LBL_Playcard.Content))
                {
                    LBL_Playcard.Foreground = Brushes.Red;
                    LBL_Playcard.Content = Convert.ToString(lowestcard);
                    CPUHand.Remove(Convert.ToString(lowestcard));
                    remainder = 0;
                }
                else if (hasgreater && lowestcard < Convert.ToByte(LBL_Playcard.Content) - 3)
                {
                    LBL_Playcard.Foreground = Brushes.Red;
                    LBL_Playcard.Content = Convert.ToString(lowestgreater);
                    CPUHand.Remove(Convert.ToString(lowestgreater));
                    remainder = 0;
                }
                else if (hasgreater && lowestcard < Convert.ToByte(LBL_Playcard.Content))
                {
                    remainder = Convert.ToByte(Convert.ToByte(LBL_Playcard.Content) - lowestcard);
                    LBL_Playcard.Foreground = Brushes.Red;
                    LBL_Playcard.Content = Convert.ToString(lowestcard);
                    CPUHand.Remove(Convert.ToString(lowestcard));
                }
                else
                {
                    if (highestcard == 0)
                    {
                        remainder = 0;
                        LBL_Playcard.Foreground = Brushes.Red;
                        LBL_Playcard.Content = "Wild";
                        CPUHand.Remove("Wild");
                    }
                    else
                    {
                        remainder = Convert.ToByte(Convert.ToByte(LBL_Playcard.Content) - highestcard);
                        if (remainder < 0)
                        {
                            remainder = 0;
                        }
                        LBL_Playcard.Foreground = Brushes.Red;
                        LBL_Playcard.Content = Convert.ToString(highestcard);
                        CPUHand.Remove(Convert.ToString(highestcard));
                    }
                }
                Trace.WriteLine($"{haswild}, {hasequal}, {hasgreater}, {highestcard}, {lowestcard}, {lowestgreater}");
                RCT_EcardV.Visibility = Visibility.Hidden;
                PrepEndTurn(true, remainder);
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
                        BTN_CardV.IsEnabled = false;
                        BTN_CardV.Visibility = Visibility.Hidden;
                        BTN_CardI.Content = PlrHand[0];
                        BTN_CardII.Content = PlrHand[1];
                        BTN_CardIII.Content = PlrHand[2];
                        BTN_CardIV.Content = PlrHand[3];
                        break;

                    case "BTN_CardII":
                        value = PlrHand[1];
                        LBL_Playcard.Foreground = Brushes.Green;
                        LBL_Playcard.Content = value;
                        PlrHand.RemoveAt(1);
                        BTN_CardV.IsEnabled = false;
                        BTN_CardV.Visibility = Visibility.Hidden;
                        BTN_CardI.Content = PlrHand[0];
                        BTN_CardII.Content = PlrHand[1];
                        BTN_CardIII.Content = PlrHand[2];
                        BTN_CardIV.Content = PlrHand[3];
                        break;

                    case "BTN_CardIII":
                        value = PlrHand[2];
                        LBL_Playcard.Foreground = Brushes.Green;
                        LBL_Playcard.Content = value;
                        PlrHand.RemoveAt(2);
                        BTN_CardV.IsEnabled = false;
                        BTN_CardV.Visibility = Visibility.Hidden;
                        BTN_CardI.Content = PlrHand[0];
                        BTN_CardII.Content = PlrHand[1];
                        BTN_CardIII.Content = PlrHand[2];
                        BTN_CardIV.Content = PlrHand[3];
                        break;

                    case "BTN_CardIV":
                        value = PlrHand[3];
                        LBL_Playcard.Foreground = Brushes.Green;
                        LBL_Playcard.Content = value;
                        PlrHand.RemoveAt(3);
                        BTN_CardV.IsEnabled = false;
                        BTN_CardV.Visibility = Visibility.Hidden;
                        BTN_CardI.Content = PlrHand[0];
                        BTN_CardII.Content = PlrHand[1];
                        BTN_CardIII.Content = PlrHand[2];
                        BTN_CardIV.Content = PlrHand[3];
                        break;

                    case "BTN_CardV":
                        value = PlrHand[4];
                        LBL_Playcard.Foreground = Brushes.Green;
                        LBL_Playcard.Content = value;
                        PlrHand.RemoveAt(4);
                        BTN_CardV.IsEnabled = false;
                        BTN_CardV.Visibility = Visibility.Hidden;
                        BTN_CardI.Content = PlrHand[0];
                        BTN_CardII.Content = PlrHand[1];
                        BTN_CardIII.Content = PlrHand[2];
                        BTN_CardIV.Content = PlrHand[3];
                        break;
                }
                turn = false;
                await PrepCPUDefend();
            }
        }
    }
}
