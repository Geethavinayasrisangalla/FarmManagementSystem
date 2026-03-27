using FarmManagement.Web.Data;
using FarmManagement.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.Web.Services
{
    {
                _context.Fields.Remove(field);
                await _context.SaveChangesAsync();
            }
        }
    }
}
