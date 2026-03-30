<<<<<<< HEAD
namespace cdpTracker_Api.DTOs
{
	public class CreateEnvelopeRequest
	{ 
		public string Code { get; set; } = string.Empty
		public decimal Amount { get; set; } 
		public int WorkerId { get; set;  }
	}
}
=======
﻿namespace cdpTracker_Api.DTOs
{
    public class CreateEnvelopeRequest
    {
        public string Code { get; set; } = string.Empty;
        public decimal Amount { get; set; } 
        public int WorkerId { get; set; }
    }
}
>>>>>>> 2e0d26a (update controller, now code is always an string instead of int)
