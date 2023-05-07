[Serializable]
    internal class Neuron
    {
        public List<Synapse> inputSynapses;
        public List<Synapse> outputSynapses;
        public double output;
        public double bias;

        public Neuron(Random random)
        {
            inputSynapses = new List<Synapse>();
            outputSynapses = new List<Synapse>();
            output = 0;
            bias = random.NextDouble() * 2 - 1;
        }

        public Neuron(Neuron copyFrom)
        {
            inputSynapses = new List<Synapse>();
            outputSynapses = new List<Synapse>();
            output = 0;
            bias = copyFrom.bias;
        }

        private double Tanh(double value)
        {
            return Math.Tanh(value);
        }

        public void Activate()
        {
            double sum = 0;
            foreach (Synapse synapse in inputSynapses)
            {
                sum += synapse.weight * synapse.inputNeuron.output;
            }
            output = Tanh(sum + bias);
        }
    }
