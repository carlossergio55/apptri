using Infraestructura.Abstract;
using Infraestructura.Models.Integracion;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server.Pages.Pages.Admin_Integracion
{
    public partial class ChoferAdmin
    {
        private bool expande = false;

        public ChoferDto _Chofer = new();
        public List<ChoferDto> _choferes = new();

        // ---------- CRUD ----------
        private async Task GetChoferes()
        {
            var res = await _Rest.GetAsync<List<ChoferDto>>("Chofer/chofer");
            if (res.State == State.Success)
                _choferes = res.Data;
            else
                _MessageShow(res.Message, State.Warning);
        }

        private async Task Save(ChoferDto dto)
        {
            _Loading.Show();
            var r = await _Rest.PostAsync<int?>("Chofer/guardar", new { Chofer = dto });
            _Loading.Hide();
            _MessageShow(r.Message, r.State);
            if (r.State != State.Success) r.Errors.ForEach(e => _MessageShow(e, State.Warning));
        }

        private async Task Update(ChoferDto dto)
        {
            _Loading.Show();
            var r = await _Rest.PutAsync<int>("Chofer", dto, dto.IdChofer);
            _Loading.Hide();
            _MessageShow(r.Message, r.State);
        }

        private async Task Eliminar(int id)
        {
            await _MessageConfirm("¿Eliminar el registro?", async () =>
            {
                var r = await _Rest.DeleteAsync<int>("Chofer", id);
                if (!r.Succeeded) _MessageShow(r.Message, State.Error);
                else
                {
                    _MessageShow(r.Message, r.State);
                    await GetChoferes();
                    StateHasChanged();
                }
            });
        }

        // ---------- Formulario ----------
        private async Task OnValidChofer(EditContext _)
        {
            if (_Chofer.IdChofer > 0)
                await Update(_Chofer);
            else
                await Save(_Chofer);

            _Chofer = new ChoferDto();
            await GetChoferes();
            ToggleExpand();
        }

        private void FormEditar(ChoferDto dto)
        {
            _Chofer = new ChoferDto
            {
                IdChofer = dto.IdChofer,
                Nombre = dto.Nombre,
                Licencia = dto.Licencia,
                Celular = dto.Celular
            };
            ToggleExpand();
        }

        private void ResetChofer() => _Chofer = new ChoferDto();
        private void ToggleExpand() => expande = !expande;

        // ---------- Init ----------
        protected override async Task OnInitializedAsync()
        {
            await GetChoferes();
        }
    }
}
