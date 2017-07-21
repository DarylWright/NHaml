using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHaml.Samples.Xamarin.Forms.Models;

namespace NHaml.Samples.Xamarin.Forms.Services
{
	public class MockDataStore : IDataStore<Item>
	{
	    private bool _isInitialized;
	    private List<Item> _items;

		public async Task<bool> AddItemAsync(Item item)
		{
			await InitializeAsync();

			_items.Add(item);

			return await Task.FromResult(true);
		}

		public async Task<bool> UpdateItemAsync(Item item)
		{
			await InitializeAsync();

			var oldItem = _items.FirstOrDefault(arg => arg.Id == item.Id);

			_items.Remove(oldItem);

			_items.Add(item);

			return await Task.FromResult(true);
		}

		public async Task<bool> DeleteItemAsync(Item item)
		{
			await InitializeAsync();

			var toDelete = _items.FirstOrDefault(arg => arg.Id == item.Id);

			_items.Remove(toDelete);

			return await Task.FromResult(true);
		}

		public async Task<Item> GetItemAsync(string id)
		{
			await InitializeAsync();

			return await Task.FromResult(_items.FirstOrDefault(s => s.Id == id));
		}

		public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
		{
			await InitializeAsync();

			return await Task.FromResult(_items);
		}

		public Task<bool> PullLatestAsync()
		{
			return Task.FromResult(true);
		}

		public Task<bool> SyncAsync()
		{
			return Task.FromResult(true);
		}

		public async Task InitializeAsync()
		{
			if (_isInitialized)
				return;

		    await Task.FromResult(true); // Does nothing but get rid of async warning; nothing else is awaitable in this method for now

			_items = new List<Item>
			{
				new Item { Id = Guid.NewGuid().ToString(), Text = "Buy some cat food", Description="The cats are hungry"},
				new Item { Id = Guid.NewGuid().ToString(), Text = "Learn F#", Description="Seems like a functional idea"},
				new Item { Id = Guid.NewGuid().ToString(), Text = "Learn to play guitar", Description="Noted"},
				new Item { Id = Guid.NewGuid().ToString(), Text = "Buy some new candles", Description="Pine and cranberry for that winter feel"},
				new Item { Id = Guid.NewGuid().ToString(), Text = "Complete holiday shopping", Description="Keep it a secret!"},
				new Item { Id = Guid.NewGuid().ToString(), Text = "Finish a todo list", Description="Done"},
			};

			_isInitialized = true;
		}
	}
}
