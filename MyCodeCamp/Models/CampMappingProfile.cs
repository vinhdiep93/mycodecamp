﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyCodeCamp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCodeCamp.Models
{
    public class CampMappingProfile : Profile
    {
        public CampMappingProfile()
        {
            CreateMap<Camp, CampModel>()
                .ForMember(c => c.StartDate, opt => opt.MapFrom(camp => camp.EventDate))
                .ForMember(c => c.EndDate, opt => opt.ResolveUsing(camp => camp.EventDate.AddDays(camp.Length - 1)))
                .ForMember(c => c.Url, opt => opt.ResolveUsing((camp, model, unused, ctx) =>
                {
                    var url = (IUrlHelper)ctx.Items["UrlHelper"];
                    return url.Link("CampGet", new { id = camp.Id });
                }));
        }
    }
}
