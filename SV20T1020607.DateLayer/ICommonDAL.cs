using System;
namespace SV20T1020607.DataLayer
{
	public interface ICommonDAL<T> where T : class

    {
        /// <summary>
        /// Tim kiem va lay du lieu duoi dang phan trang
        /// </summary>
        /// <param name="page"></param>trang can hien thi
        /// <param name="pageSize"></param>so dong tren moi trang
        /// <param name="searchValue"></param>Gia tri tim kiem
        /// <returns></returns>
        IList<T> List(int page = 1, int pageSize = 0, String searchValue = "");

        /// <summary>
        /// Dem so luong dong du lieu tim duoc
        /// </summary>
        /// <param name="searchValue"></param>Gia tri tim kiem (Chuoi rong neu lay toan bo du lieu) 
        /// <returns></returns>
        int Count(String searchValue = " ");
        /// <summary>
        /// Lay 1 ban ghi dua tren id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T? Get(int id);

        /// <summary>
        /// Bo sung du lieu vao CSDL(Tra ve gia tri <=0 neu loi )
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int Add(T data);
        /// <summary>
        /// Sua du lieu
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Update(T data);
        /// <summary>
        /// Xoa du lieu  dua vao id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Delete(int id);
        /// <summary>
        /// Kiem tra  xem 1 ban ghi co ma id hien dang co duoc su dung boi cac bang khac hay khong(Co du lieu lien quan hay khong?)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool IsUsed(int id);

        
        
    }
}

