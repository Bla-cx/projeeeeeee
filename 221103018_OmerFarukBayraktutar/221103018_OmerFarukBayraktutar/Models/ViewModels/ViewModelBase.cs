using System.Collections.Generic;

namespace _221103018_OmerFarukBayraktutar.ViewModels
{
    /// <summary>
    /// View modeller için temel sınıf
    /// CS8618 uyarılarını azaltmak amacıyla varsayılan değerler içerir
    /// </summary>
    public class ViewModelBase
    {
        // String özelliklerin varsayılan değerlerini ayarlama
        protected static string EmptyString => string.Empty;
        protected static int DefaultNumber => 0;
        protected static IList<T> EmptyList<T>() => new List<T>();
        
        /// <summary>
        /// Model durumunu kontrol etmek için yardımcı metot
        /// </summary>
        public virtual bool IsValid()
        {
            return true; // Türetilmiş sınıflar override edebilir
        }
    }
    
    /// <summary>
    /// Koleksiyon içeren view modeller için temel sınıf
    /// </summary>
    public abstract class CollectionViewModelBase<T> : ViewModelBase
    {
        private ICollection<T> _items = new List<T>();
        
        public ICollection<T> Items 
        { 
            get => _items; 
            set => _items = value ?? new List<T>(); 
        }
        
        public bool HasItems => Items.Count > 0;
        public int ItemCount => Items.Count;
    }
} 