using Aplicacion.DTOs.Clasificador;
using Aplicacion.DTOs.Integracion;
using Aplicacion.DTOs.Segurity;
using AutoMapper;
using Dominio.Entities;
using Dominio.Entities.Integracion;
using Dominio.Entities.Seguridad;
using System;



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
            CreateMap<Viaje, ViajeDto>();
            CreateMap<Boleto, BoletoDto>();
            CreateMap<Encomienda, EncomiendaDto>()
            .ForMember(dest => dest.CodigoGuia, opt => opt.MapFrom(src => src.Guia.Codigo));
            CreateMap<Pago, PagoDto>();
            CreateMap<Parada, ParadaDto>();
            CreateMap<RutaParada, RutaParadaDto>();
            CreateMap<TarifaTramo, TarifaTramoDto>();

            CreateMap<GuiaCarga, GuiaCargaDto>().ReverseMap();


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
            CreateMap<ViajeDto, Viaje>();
            CreateMap<BoletoDto, Boleto>();
            CreateMap<EncomiendaDto, Encomienda>();
            CreateMap<PagoDto, Pago>();
            CreateMap<ParadaDto, Parada>();
            CreateMap<RutaParadaDto, RutaParada>();
            CreateMap<TarifaTramoDto, TarifaTramo>();
            CreateMap<EncomiendaDto, Encomienda>();


            #endregion
        }
    }
}
