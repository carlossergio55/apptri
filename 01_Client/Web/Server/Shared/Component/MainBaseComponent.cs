using Infraestructura.Abstract;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;

namespace Server.Shared.Component
{
  
    public class MainBaseComponent : ComponentBase
    {
        [Inject] private ISnackbar Snackbars { get; set; }
        [Inject] private IDialogService DialogService { get; set; } 

        public void _MessageShow(string Message, State state)
        {
            switch (state)
            {
                case State.Success:
                    Snackbars.Add(Message, Severity.Success);
                    break;
                case State.Warning:
                    Snackbars.Add(Message, Severity.Warning);
                    break;
                case State.Error:
                    Snackbars.Add(Message, Severity.Error);
                    break;
                case State.NoData:
                    Snackbars.Add(Message, Severity.Info);
                    break;
                default:
                    Snackbars.Add(Message, Severity.Normal);
                    break;
            }
        }

        public void _DialogShow(string Message, State state)
        {
            var parameters = new DialogParameters();
            parameters.Add("message", Message);
            parameters.Add("state", state);
            DialogService.Show<SiabysDialog>("", parameters);
        }

        public async Task _MessageConfirm(string Message, System.Action aceptable)
        {
            var parameters = new DialogParameters();
            parameters.Add("message", Message);
            var dialog = DialogService.Show<SiabysDialogConfirm>("", parameters);
            var result = await dialog.Result;

            if (!result.Cancelled)
            {
                aceptable();
            }
        }

        public async Task _MessageConfirm(string Message, System.Action aceptable, System.Action cancelable)
        {
            var parameters = new DialogParameters();
            parameters.Add("message", Message);
            var dialog = DialogService.Show<SiabysDialogConfirm>("", parameters);
            var result = await dialog.Result;

            if (!result.Cancelled)
            {
                aceptable();
            }
            else {
                cancelable();
            }
        }

    }
}
