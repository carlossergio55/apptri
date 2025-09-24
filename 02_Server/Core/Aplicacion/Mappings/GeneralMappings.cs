using Aplicacion.DTOs.Clasificador;
using Aplicacion.DTOs.Integracion;
using Aplicacion.DTOs.Segurity;
using AutoMapper;
using Dominio.Entities;
using Dominio.Entities.Integracion;
using Dominio.Entities.Seguridad;
using System;
using System.Globalization;



namespace Aplicacion.Mappings
{
    public class GeneralMappings : Profile
    {

        public GeneralMappings()
        {
            //TODO: Agregar aqui el registro de mapeo para obtenion de consultas  direccion  EntidadDominio --> ModeloDto
            #region QueryDto
            CreateMap<SegvUsuario, SegUsuarioDto>();
            CreateMap<SegUsuario, SeUsuarioDto>();
            CreateMap<SegvMenuobjetos, UserMenuDto>();


            CreateMap<GenClasificador, GenClasificadorDto>();
            CreateMap<GenClasificadortipo, GenClasificadortipoDto>();
            CreateMap<Bus, BusDto>();
            CreateMap<Chofer, ChoferDto>();
            CreateMap<Cliente, ClienteDto>();
            CreateMap<Ruta, RutaDto>();
            CreateMap<Usuario, UsuarioDto>();
            CreateMap<Horario, HorarioDto>();
            CreateMap<Asiento, AsientoDto>();
            CreateMap<Viaje, ViajeDto>()
                .ForMember(d => d.HoraSalida,
                    o => o.MapFrom(s => s.HoraSalida.ToString(@"hh\:mm\:ss")))
                .ForMember(d => d.Estado,
                    o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.Estado) ? "PROGRAMADO" : s.Estado))
                .ForMember(d => d.Direccion,
                    o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.Direccion) ? "IDA" : s.Direccion));

            CreateMap<Boleto, BoletoDto>();
            CreateMap<Encomienda, EncomiendaDto>();
            CreateMap<Pago, PagoDto>();
            CreateMap<Parada, ParadaDto>();
            CreateMap<RutaParada, RutaParadaDto>();
            CreateMap<TarifaTramo, TarifaTramoDto>();
            CreateMap<Sucursal, SucursalDto>().ReverseMap();
            // Entidad -> DTO público de tracking
            CreateMap<Encomienda, EncomiendaPublicDto>()
                .ForMember(d => d.OrigenParadaNombre, o => o.MapFrom(s => s.OrigenParada != null ? s.OrigenParada.Nombre : null))
                .ForMember(d => d.DestinoParadaNombre, o => o.MapFrom(s => s.DestinoParada != null ? s.DestinoParada.Nombre : null))
                .ForMember(d => d.FechaViaje, o => o.MapFrom(s => s.Viaje != null ? (DateTime?)s.Viaje.Fecha : null))
                .ForMember(d => d.HoraSalida, o => o.MapFrom(s => s.Viaje != null ? s.Viaje.HoraSalida.ToString(@"hh\:mm") : null))
                .ForMember(d => d.IdRuta, o => o.MapFrom(s => s.Viaje != null ? s.Viaje.IdRuta : 0));



            #endregion
            //TODO: Agregar aqui el registro de mapeo para ejecucion de comandos  direccion  ModeloDto --> EntidadDominio Ej. : CreateMap<ProductoDto, CapProducto>();

            #region Commands
            CreateMap<SeUsuarioDto, SegUsuario>();
            //CreateMap<ConInventarioDto, ConInventario>();
            CreateMap<GenClasificadorDto, GenClasificador>();
            CreateMap<GenClasificadortipoDto, GenClasificadortipo>();
            CreateMap<BusDto, Bus>();
            CreateMap<ChoferDto, Chofer>();
            CreateMap<ClienteDto, Cliente>();
            CreateMap<RutaDto, Ruta>();
            CreateMap<UsuarioDto, Usuario>();
            CreateMap<HorarioDto, Horario>();
            CreateMap<AsientoDto, Asiento>();
            CreateMap<ViajeDto, Viaje>()
              .ForMember(d => d.HoraSalida,
                  o => o.MapFrom(s => ParseHora(s.HoraSalida)))
              .ForMember(d => d.Estado,
                  o => o.MapFrom(s => (s.Estado ?? "PROGRAMADO").ToUpperInvariant()))
              .ForMember(d => d.Direccion,
                  o => o.MapFrom(s => (s.Direccion ?? "IDA").ToUpperInvariant()));
            CreateMap<BoletoDto, Boleto>()
    .ForMember(d => d.IdBoleto, o => o.Ignore()) // lo genera la BD
    .ForMember(d => d.Estado,
        o => o.MapFrom(s => (s.Estado ?? "BLOQUEADO").ToUpperInvariant()));
            CreateMap<EncomiendaDto, Encomienda>()
               .ForMember(d => d.Guiacarga, o => o.Ignore());
            CreateMap<PagoDto, Pago>();
            CreateMap<ParadaDto, Parada>();
            CreateMap<RutaParadaDto, RutaParada>();
            CreateMap<TarifaTramoDto, TarifaTramo>();
            
            CreateMap<SucursalDto, Sucursal>();

            #endregion
        }

        private static TimeSpan ParseHora(string? h)
        {
            if (string.IsNullOrWhiteSpace(h)) return TimeSpan.Zero;
            return TimeSpan.TryParseExact(h,
                new[] { @"hh\:mm", @"hh\:mm\:ss" },
                CultureInfo.InvariantCulture, out var ts)
                ? ts : TimeSpan.Zero;
        }
    }
}
