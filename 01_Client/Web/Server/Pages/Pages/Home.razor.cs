//using Infraestructura.Abstract;
//using Server.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;


//namespace Server.Pages.Pages
//{
//    public partial class Home
//    {
//        public List<Order> Orders { get; set; }
//        protected override void OnInitialized()
//        {
//            Orders = Enumerable.Range(1, 75).Select(x => new Order()
//            {
//                OrderID = 1000 + x,
//                CustomerID = (new string[] { "ALFKI", "ANANTR", "ANTON", "BLONP", "BOLID" })[new Random().Next(5)],
//                Freight = 2.1 * x,
//                OrderDate = DateTime.Now.AddDays(-x),
//            }).ToList();
//        }
//        public class Order
//        {
//            public int? OrderID { get; set; }
//            public string CustomerID { get; set; }
//            public DateTime? OrderDate { get; set; }
//            public double? Freight { get; set; }
//        }

//        async Task buttonClick()
//        {

//            //var authSate = await authenticationState;
//            //var usuario = authSate.User;

//            //_Loading.Show();

//            // _MessageShow($"Nombre :  { usuario.Identity.Name }", State.Error);
//            //_MessageShow("Felipe Viscarra", State.Success);
//            //_MessageShow("Felipe Viscarra", State.Warning);            
//            //_MessageShow("Felipe Viscarra", State.NoData);            

//            //await _MessageConfirm("Esta seguro de continuar con el registro", () =>
//            //{                               
//            //    _MessageShow("Este es un error accion", State.Error);
//            //});

//            //_DialogShow("Este es un error comun lorem oadfasdf asdfasdfas asdf", State.Error);

//            //if (usuario.Identity.IsAuthenticated)
//            //{
//            //    Console.WriteLine(usuario.Identity.Name);
//            //}
//            //else
//            //{
//            //}


//            try
//            {


//                ///: test llamada con varios parametros por querstrig : controlador/accion?id=125445&paramtro=ldkdfj
//                ///obligado utilizar un modelo
//                var vrange = new GetRange
//                {
//                    FechaInicio = new System.DateTime(2021, 6, 1),
//                    FechaFinal = new System.DateTime(2021, 6, 30)
//                };
//                var result1 = await _Rest.GetAsyncFromQuery<List<FacturaModel>>("factura", vrange);
//                if (result1.State != State.Success)
//                {
//                    _MessageShow(result1.Message, State.Error);
//                }

//                /////: test llamada con varios parametros por el paht :   controlador/acciion/parametro1/parametro2
//                /////si hay mas de un parametro utilizar un mmodelo
//                //int id = 142734;
//                //var result = await _Rest.GetAsyncFromPath<FacturaModel>("factura", id);
//                //if (result.State != State.Success)
//                //{
//                //    _MessageShow(result.Message, State.Error);
//                //}

//                ////: EJEMPLO DE POST INSERTAR REGISTRO
//                //FacturaCommand fact = new()
//                //{
//                //    IdfactFacturadosificacion = 9,
//                //    CodigoControl = "145-2545asdf-asf454",
//                //    NroFactura = 12245,
//                //    FechaFactura = new DateTime(2021, 10, 13),
//                //    IdfactPlanillaentidad = 6063,
//                //    IdcEstadofactura = 32,
//                //    NitCedula = "48395952",
//                //    Nombre = "TEST TEST TEST"
//                //};
//                //var vresupost = await _Rest.PostAsync<int>("factura", fact);
//                //if (!vresupost.Succeeded)
//                //{
//                //    _MessageShow(vresupost.Message, State.Error);
//                //}

//                //FacturaUpdate fact = new()
//                //{
//                //    IdfactFacturacabecera = 163888,
//                //    FechaFactura = new DateTime(2021, 9, 1),
//                //    NitCedula = "1010101010",
//                //    Nombre = "EL nuevo solitario"
//                //};
//                //var result = await _Rest.PutAsync<int>("factura", fact, 163888);


//                ////:ejemplo Delete
//                //var resul = await _Rest.DeleteAsync<int>("factura", 163888);


//            }
//            catch (System.Exception e)
//            {
//                _MessageShow(e.Message, State.Error);
//            }
//        }
//    }
//}
