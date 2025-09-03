using Infraestructura.Abstract;
using Infraestructura.Models.Integracion;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server.Pages.Pages.Admin_Integracion
{
    public partial class ParadaAdmin
    {
        private bool expande = false;

        public ParadaDto _Parada = new();
        public List<ParadaDto> _paradas = new();

        // ---------- CRUD ----------
        private async Task GetParadas()
        {
            var res = await _Rest.GetAsync<List<ParadaDto>>("Parada/parada");
            if (res.State == State.Success)
                _paradas = res.Data;
            else
                _MessageShow(res.Message, State.Warning);
        }

        private async Task Save(ParadaDto dto)
        {
            _Loading.Show();
            var r = await _Rest.PostAsync<int?>("Parada/guardar", new { Parada = dto });
            _Loading.Hide();
            _MessageShow(r.Message, r.State);
            if (r.State != State.Success)
                r.Errors.ForEach(e => _MessageShow(e, State.Warning));
        }

        private async Task Update(ParadaDto dto)
        {
            _Loading.Show();
            var r = await _Rest.PutAsync<int>("Parada", dto, dto.IdParada);
            _Loading.Hide();
            _MessageShow(r.Message, r.State);
        }

        private async Task Eliminar(int id)
        {
            await _MessageConfirm("¿Eliminar la parada?", async () =>
            {
                var r = await _Rest.DeleteAsync<int>("Parada", id);
                if (!r.Succeeded)
                    _MessageShow(r.Message, State.Error);
                else
                {
                    _MessageShow(r.Message, r.State);
                    await GetParadas();
                    StateHasChanged();
                }
            });
        }

        // ---------- Form ----------
        private async Task OnValidParada(EditContext _)
        {
            if (_Parada.IdParada > 0)
                await Update(_Parada);
            else
                await Save(_Parada);

            _Parada = new ParadaDto();
            await GetParadas();
            ToggleExpand();
        }

        private void FormEditar(ParadaDto dto)
        {
            _Parada = new ParadaDto
            {
                IdParada = dto.IdParada,
                Nombre = dto.Nombre
            };
            ToggleExpand();
        }

        private void ResetParada() => _Parada = new ParadaDto();
        private void ToggleExpand() => expande = !expande;

        // ---------- Init ----------
        protected override async Task OnInitializedAsync()
        {
            await GetParadas();
        }
    }
}
