﻿using Domain.Interfaces.Infra;
using X.PagedList;

namespace Infra.Data.Support.Pagination
{
    public class PaginationService : IPagination
    {
        /// <summary>
        /// Realiza a paginação de uma lista de dados genéricos.
        /// </summary>
        /// <typeparam name="T">O tipo dos dados a serem paginados.</typeparam>
        /// <param name="data">A lista de dados a ser paginada.</param>
        /// <param name="page">O número da página a ser exibida. Se nulo, a primeira página será exibida por padrão.</param>
        /// <returns>Um objeto IPagedList contendo os dados da página atual e as informações de paginação.</returns>
        public async Task<IPagedList<T>> PaginationDataAsync<T>(IEnumerable<T> data, int? page)
        {
            int pageNumber = page ?? 1;
            const int pageSize = 15;

            var pagedList = data.ToPagedList(pageNumber, pageSize);

            return pagedList;
        }
    }
}
