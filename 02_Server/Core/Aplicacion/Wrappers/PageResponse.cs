namespace Aplicacion.Wrappers
{
    public class PageResponse<T> : Response<T>
    {

        public int PageNumber { get; set; }
        public int PageZise { get; set; }

        public PageResponse(T data, int pageNumber, int pageZise)
        {
            PageNumber = pageNumber;
            PageZise = pageZise;
            Message = null;
            Succeeded = true;
            Errors = null;
            Data = data;
        }

    }
}
