﻿using Microsoft.EntityFrameworkCore;
using SalesMVC.Data;
using SalesMVC.Models;
using SalesMVC.Services.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalesMVC.Services {
    public class SellerService {
        // DBContext dependency
        private readonly SalesMVCContext _context;

        // Constructor with dependency injection
        public SellerService(SalesMVCContext context) {
            _context = context;
        }

        // Methods
        public async Task<List<Seller>> FindAllAsync() {
            return await _context.Seller.ToListAsync();
        }

        public async Task InsertAsync(Seller seller) {
            _context.Add(seller);
            await _context.SaveChangesAsync();
        }

        public async Task<Seller> FindByIdAsync(int id) {
            return await _context.Seller.Include(s => s.Department).FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task RemoveAsync(int id) {
            try {
                Seller seller = await _context.Seller.FindAsync(id);
                _context.Seller.Remove(seller);
                await _context.SaveChangesAsync();
            } catch(DbUpdateException e) {
                throw new IntegrityException("Não é possível excluir este vendedor, pois ele possui vendas cadastradas!");
            }
        }

        public async Task UpdateAsync(Seller seller) {
            bool hasAny = await _context.Seller.AnyAsync(s => s.Id == seller.Id);
            if (!hasAny) throw new NotFoundException("Id não encontrado!");
            try {
                _context.Update(seller);
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException e) {
                throw new DBConcurrencyException(e.Message);
            }
        }
    }
}
