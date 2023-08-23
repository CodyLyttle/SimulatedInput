using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using SimulatedInput.library;
using SimulatedInput.library.wrapper;

namespace SimulatedInput
{
    public partial class MainWindow : Window
    {
        private readonly InputSender _inputSender = new();

        public MainWindow()
        {
            InitializeComponent();
            SendInputButton.Click += SendInputButtonOnClick;
        }

        private async void SendInputButtonOnClick(object sender, RoutedEventArgs e)
        {
            await SendDelayedInput(TimeSpan.FromSeconds(3), new ushort[]
            {
                0x48, 0x45, 0x4C, 0x4C, 0x4F // 'hello'
            });
        }

        private async Task SendDelayedInput(TimeSpan delayTime, IEnumerable<ushort> virtualKeyCodes)
        {
            await Task.Delay(delayTime);

            List<IInputAction> inputActions = new();
            foreach (ushort vk in virtualKeyCodes)
            {
                inputActions.Add(new VirtualKeyInputAction(vk, false));
                inputActions.Add(new VirtualKeyInputAction(vk, true));
            }

            _inputSender.SendKeystrokes(inputActions.ToArray());
        }
    }
}