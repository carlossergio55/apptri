// Server/CompraFlowState.cs
namespace Server
{
    // 1=Buscar, 2=Asientos, 3=Datos, 4=Pago
    public class CompraFlowState
    {
        public int MaxStepReached { get; private set; } = 1;
        public bool CanEnter(int step) => step <= MaxStepReached;
        public void AdvanceTo(int step)
        {
            if (step > MaxStepReached) MaxStepReached = step;
        }
        public void Reset() => MaxStepReached = 1;
    }
}
