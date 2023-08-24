using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SimulatedInput.Library;
using SimulatedInput.Library.Enum;
using SimulatedInput.Library.Wrapper;

namespace SimulatedInput
{
    public partial class MainWindow : Window
    {
        private static readonly TimeSpan DelayTime = TimeSpan.FromSeconds(3);
        private readonly InputSender _inputSender = new();

        public MainWindow()
        {
            InitializeComponent();
            SendInputButton.Click += SendInputButtonOnClick;
            SendInputTextBox.PreviewKeyDown += SendInputTextBoxOnPreviewKeyDown;
        }

        private async void SendInputButtonOnClick(object sender, RoutedEventArgs e)
        {
            await SendDelayedInput(DelayTime, new VirtualKeyCode[]
            {
                VirtualKeyCode.H, 
                VirtualKeyCode.E, 
                VirtualKeyCode.L, 
                VirtualKeyCode.L, 
                VirtualKeyCode.O
            });
        }

        private async Task SendDelayedInput(TimeSpan delayTime, IEnumerable<VirtualKeyCode> virtualKeyCodes)
        {
            await Task.Delay(delayTime);

            List<IInputAction> inputActions = new();
            foreach (VirtualKeyCode keyCode in virtualKeyCodes)
            {
                inputActions.Add(new VirtualKeyInputAction(keyCode, false));
                inputActions.Add(new VirtualKeyInputAction(keyCode, true));
            }

            _inputSender.SendKeystrokes(inputActions.ToArray());
        }
        

        private async void SendInputTextBoxOnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            await SendDelayedText(DelayTime, SendInputTextBox.Text);
            SendInputTextBox.Clear();
        }

        private async Task SendDelayedText(TimeSpan delayTime, string text)
        {
            await Task.Delay(delayTime);
            _inputSender.SendText(text);
        }
    }
}