using System;

namespace Infraestructura.Component
{
    public class Loading
    {
        public bool vVisble { get; set; } = false;

        public Action CambiarEstado;

        public void Show()
        {
            this.vVisble = true;
            CambiarEstado?.Invoke();
        }

        public void Hide()
        {
            this.vVisble = false;
            CambiarEstado?.Invoke();
        }
    }
}
