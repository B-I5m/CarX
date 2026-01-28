using CarX.Domain.Entities;
using CarX.Application.DTOs;
using CarX.Application.Interfaces;
using CarX;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace CarX.Application.Interfaces;
public interface IBrandService
{
    Task<IEnumerable<Brand>> GetAllAsync();
    Task<Brand?> GetByIdAsync(long id);
    Task<Brand> CreateAsync(Brand brand);
    Task<bool> UpdateAsync(Brand brand);
    Task<bool> DeleteAsync(long id);
}