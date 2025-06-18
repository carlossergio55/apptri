using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRest.Model
{
	public class Efficiency
	{
		public decimal Calculated { get; set; }
	}
	
    public class EfficiencyRoot
    {
		public EfficiencyEnvelope Envelope { get; set; }
    }

	public class EfficiencyResult
	{
		public decimal Calculated { get; set; }
	}

	public class EfficiencyResponse
	{
		public EfficiencyResult EfficiencyResult { get; set; }
	}

	public class EfficiencyBody
	{
		public EfficiencyResponse EfficiencyResponse { get; set; }
	}

	public class EfficiencyEnvelope
	{
		public EfficiencyBody Body { get; set; }
	}
}
