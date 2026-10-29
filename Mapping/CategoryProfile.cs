using System;
using ApiEcommerce.Models.Dtos;
using AutoMapper;

namespace ApiEcommerce.Mapping;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category,CategoryDto>().ReverseMap();
        CreateMap<Category,CreateCategoryDto>().ReverseMap();
    }

}

//Note:
// Automapper es una librería que permite mapear objetos de un tipo a otro, lo que facilita la transferencia de datos entre diferentes capas de una aplicación. En este caso, se está utilizando para mapear entre la entidad Category y los DTOs CategoryDto y CreateCategoryDto.
// Para terminar de entender el código, es importante conocer la estructura de la entidad Category y los DTOs CategoryDto y CreateCategoryDto. La entidad Category representa una categoría en la base de datos, mientras que los DTOs son objetos que se utilizan para transferir datos entre la capa de presentación y la capa de negocio. En este caso, se está utilizando AutoMapper para mapear entre estos objetos, lo que permite una transferencia de datos más eficiente y menos propensa a errores.