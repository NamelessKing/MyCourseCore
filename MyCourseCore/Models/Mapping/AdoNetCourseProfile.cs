﻿using AutoMapper;
using MyCourseCore.Models.Mapping.Resolvers;
using MyCourseCore.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MyCourseCore.Models.Mapping
{
    public class AdoNetCourseProfile : Profile
    {
        public AdoNetCourseProfile()
        {
            CreateMap<DataRow, CourseViewModel>()
                .ForMember(viewModel => viewModel.Id, config => config.MapFrom(new IdResolver()))
                .ForMember(viewModel => viewModel.CurrentPrice, config => config.MapFrom(new MoneyResolver("CurrentPrice")))
                .ForMember(viewModel => viewModel.FullPrice, config => config.MapFrom(new MoneyResolver("FullPrice")))
                .ForAllOtherMembers(config => config.MapFrom(new DefaultResolver(config.DestinationMember.Name)));

            CreateMap<DataRow, CourseDetailViewModel>()
                //.IncludeBase<DataRow, CourseViewModel>()
                .ForMember(viewModel => viewModel.Id, config => config.MapFrom(new IdResolver()))
                .ForMember(viewModel => viewModel.CurrentPrice, config => config.MapFrom(new MoneyResolver("CurrentPrice")))
                .ForMember(viewModel => viewModel.FullPrice, config => config.MapFrom(new MoneyResolver("FullPrice")))
                .ForMember(viewModel => viewModel.Lessons, config => config.Ignore())
                .ForMember(viewModel => viewModel.TotalCourseDuration, config => config.Ignore())
                .ForAllOtherMembers(config => config.MapFrom(new DefaultResolver(config.DestinationMember.Name)));

            CreateMap<DataRow, LessonViewModel>()
                .ForMember(viewModel => viewModel.Id, config => config.MapFrom(new IdResolver()))
                .ForMember(viewModel => viewModel.Duration, config => config.MapFrom(new TimeSpanResolver("Duration")))
                .ForAllOtherMembers(config => config.MapFrom(new DefaultResolver(config.DestinationMember.Name)));
        }
    }
}
