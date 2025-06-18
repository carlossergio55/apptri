using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRest.Model
{
	public class Speedup
	{
		public decimal Calculated { get; set; }
	}
	
    public class SpeedupRoot
    {
		public SpeedupEnvelope Envelope { get; set; }
    }

	public class SpeedupResult
	{
		public decimal Calculated { get; set; }
	}

	public class SpeedupResponse
	{
		public SpeedupResult SpeedupResult { get; set; }
	}

	public class SpeedupBody
	{
		public SpeedupResponse SpeedupResponse { get; set; }
	}

	public class SpeedupEnvelope
	{
		public SpeedupBody Body { get; set; }
	}
}
