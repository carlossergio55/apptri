namespace Aplicacion.Parameters
{
    public class RequestParameter
    {
        public int PageNumber { get; set; }
        public int PageZise { get; set; }

        public RequestParameter()
        {
            PageNumber = 1;
            PageZise = 10;
        }

        public RequestParameter(int pageNumber, int pageZise)
        {
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageZise = pageZise > 10 ? 10 : pageZise;
        }
    }
}
