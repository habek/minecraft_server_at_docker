using Docker.DotNet.Models;
using MinecraftServerManager.Communication.Docker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftServerManager.Minecraft
{
	public class MinecraftServer
	{
		private string _iD;
		private DockerHost? _dockerHost;

		public MinecraftServer(string iD)
		{
			_iD = iD;
		}

		public string State { get; private set; } = "Unknown";
		public string Status { get; private set; } = "Unknown";
		public string Id { get; internal set; }

		public override string ToString()
		{
			return _iD;
		}

		internal void RefreshState(ContainerListResponse container)
		{
			Id = container.ID;
			State = container.State;
			Status = container.Status;
		}
	}
}
