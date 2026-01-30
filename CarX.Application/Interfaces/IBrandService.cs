using CarX.Domain.Entities;
using CarX.Application.DTOs;
using CarX.Application.Interfaces;
using CarX;
using AutoMapper;
using CarX.Application.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CarX.Application.Interfaces;
public interface IBrandService
{
    Task<IEnumerable<Brand>> GetAllAsync();
    Task<Brand?> GetByIdAsync(long id);
    Task<Brand> CreateAsync(Brand brand);
    // CarX.Application.Interfaces/IBrandService.cs
    Task<bool> UpdateBrandAsync(long id, BrandUpdateRequest request);
    Task<bool> DeleteAsync(long id);
}