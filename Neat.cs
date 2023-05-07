    [Serializable]
    internal class NEAT
    {
        public List<Neuron> neurons = new List<Neuron>();
        public List<Synapse> synapses = new List<Synapse>();
        public List<Neuron> inputNeurons = new List<Neuron>();
        public List<Neuron> outputNeurons = new List<Neuron>();
        public List<List<Neuron>> layers = new List<List<Neuron>>();
        
        public NEAT(Random random, int inputNeuronCount, int outputNeuronCount)
        {
            initNeurons(random, inputNeuronCount, outputNeuronCount);
            initWeights(random);
        }
        
        public NEAT(Random random, NEAT copyFrom, int inputNeuronCount, int outputNeuronCount)
        {
            if (copyFrom == null)
            {
                initNeurons(random, inputNeuronCount, outputNeuronCount);
                initWeights(random);
                return;
            }
            // serialize and deserialize to copy
            using (MemoryStream memoryStream = new MemoryStream())
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, copyFrom);
                memoryStream.Position = 0;
                NEAT copy = (NEAT)binaryFormatter.Deserialize(memoryStream);
                neurons = copy.neurons;
                synapses = copy.synapses;
                inputNeurons = copy.inputNeurons;
                outputNeurons = copy.outputNeurons;
                layers = copy.layers;
            }
        }
        
        public void Save()
        {
            using (FileStream fileStream = new FileStream("neat.dat", FileMode.Create))
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(fileStream, this);
            }
        }

        public void Load()
        {
            using (FileStream fileStream = new FileStream("neat.dat", FileMode.Open))
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                NEAT copy = (NEAT)binaryFormatter.Deserialize(fileStream);
                neurons = copy.neurons;
                synapses = copy.synapses;
                inputNeurons = copy.inputNeurons;
                outputNeurons = copy.outputNeurons;
                layers = copy.layers;
            }
        }

        public void Mutate(Random random)
        {
            double chance = random.NextDouble();
            if (chance < 0.8)
            {
                MutateWeights(random);
            }
            else if (chance < 0.9)
            {
                MutateAddNeuron(random);
            }
            else
            {
                MutateAddSynapse(random);
            }
        }

        private void MutateAddSynapse(Random random)
        {
            Neuron neuron1 = neurons[random.Next(neurons.Count)];
            Neuron neuron2 = neurons[random.Next(neurons.Count)];
            if (neuron1 == neuron2)
            {
                return;
            }

            Synapse synapse = new Synapse(random, neuron1, neuron2);
        }

        private void MutateAddNeuron(Random random)
        {
            Synapse synapse = synapses[random.Next(synapses.Count)];
            int layerIndex = 0;
            for (int i = 0; i < layers.Count; i++)
            {
                if (layers[i].Contains(synapse.outputNeuron))
                {
                    layerIndex = i;
                    break;
                }
            }
            
            List<Neuron> newLayer = new List<Neuron>();
            Neuron neuron = new Neuron(random);
            newLayer.Add(neuron);
            Synapse synapse1 = new Synapse(random, synapse.inputNeuron, neuron);
            Synapse synapse2 = new Synapse(random, neuron, synapse.outputNeuron);
            neurons.Add(neuron);
            synapses.Add(synapse1);
            synapses.Add(synapse2);
            layers.Insert(layerIndex, newLayer);
            synapses.Remove(synapse);
        }

        private void MutateWeights(Random random)
        {
            foreach (Synapse synapse in synapses)
            {
                synapse.weight += random.NextDouble() * 2 - 1;
            }
        }

        private void initNeurons(Random random, int inputNeuronCount, int outputNeuronCount)
        {
            for (int i = 0; i < inputNeuronCount; i++)
            {
                Neuron neuron = new Neuron(random);
                inputNeurons.Add(neuron);
                neurons.Add(neuron);
            }
            for (int i = 0; i < outputNeuronCount; i++)
            {
                Neuron neuron = new Neuron(random);
                outputNeurons.Add(neuron);
                neurons.Add(neuron);
            }
            layers.Add(inputNeurons);
            layers.Add(outputNeurons);
        }

        private void initWeights(Random random)
        {
            for (int i = 0; i < inputNeurons.Count; i++)
            {
                for (int j = 0; j < outputNeurons.Count; j++)
                {
                    Synapse synapse = new Synapse(random, inputNeurons[i], outputNeurons[j]);
                    synapses.Add(synapse);
                }
            }
        }

        public List<double> getOutput(List<double> inputValues)
        {
            // if not same input count as neuron count
            if (inputValues.Count != layers[0].Count)
            {
                throw new Exception("Input values must match input neuron count");
            }
            // set input values
            for (int i = 0; i < inputValues.Count; i++)
            {
                layers[0][i].output = inputValues[i];
            }
            // activate neurons
            foreach (List<Neuron> layer in layers)
            {
                foreach (Neuron neuron in layer)
                {
                    neuron.Activate();
                }
            }
            // get output values
            List<double> outputValues = new List<double>();
            // for each output neuron
            foreach (Neuron neuron in layers[layers.Count - 1])
            {
                outputValues.Add(neuron.output);
            }
            // return output values
            return outputValues;
        }
    }
