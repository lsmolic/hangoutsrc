using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Client.Gui
{
	public interface IPagination
	{
		void NextPage();
		void PreviousPage();
		void LastPage();
		void FirstPage();
		void SetPagination(int itemsPerPage, int pageNumber);
		int GetTotalPages();
		int CurrentPage { get; }
	}
}
