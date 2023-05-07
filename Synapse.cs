[Serializable]
    internal class Synapse
    {
        public Neuron inputNeuron;
        public Neuron outputNeuron;
        public double weight;

        public Synapse(Random random, Neuron inputNeuron, Neuron outputNeuron)
        {
            this.inputNeuron = inputNeuron;
            this.outputNeuron = outputNeuron;
            inputNeuron.outputSynapses.Add(this);
            outputNeuron.inputSynapses.Add(this);
            weight = random.NextDouble() * 2 - 1;
        }
    }
