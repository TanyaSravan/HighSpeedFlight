import torch
import mlagents
from mlagents_envs.base_env import ActionTuple
from mlagents_envs.environment import UnityEnvironment as UE
from PPO import PPO
import numpy as np

max_ep_len = 1000                   # max timesteps in one episode
max_training_timesteps = int(3e6)   # break training loop if timeteps > max_training_timesteps

print_freq = max_ep_len * 10        # print avg reward in the interval (in num timesteps)
log_freq = max_ep_len * 2           # log avg reward in the interval (in num timesteps)
save_model_freq = int(1e5)          # save model frequency (in num timesteps)

action_std = 0.6                    # starting std for action distribution (Multivariate Normal)
action_std_decay_rate = 0.05        # linearly decay action_std (action_std = action_std - action_std_decay_rate)
min_action_std = 0.1                # minimum action_std (stop decay after action_std <= min_action_std)
action_std_decay_freq = int(2.5e5)

update_timestep = max_ep_len * 4  # update policy every n timesteps
K_epochs = 80  # update policy for K epochs in one PPO update

eps_clip = 0.2  # clip parameter for PPO
gamma = 0.99  # discount factor

lr_actor = 0.0003  # learning rate for actor network
lr_critic = 0.001  # learning rate for critic network


env = UE(file_name='Drone Controller', seed=1, side_channels=[])

env.reset()

behavior_name = list(env.behavior_specs)[0]
print(f"Name of the behavior : {behavior_name}")
spec = env.behavior_specs[behavior_name]


print("Number of observations : ", len(spec.observation_specs))

if spec.action_spec.continuous_size > 0:
  has_continuous = True
  print("Action space is continuous.")
else:
  has_continuous = False
  print("Action space is discrete.")

# This should be outside the conditional block
print("Check complete.")

decision_steps, terminal_steps = env.get_steps(behavior_name)
print(decision_steps.obs)

state_dim = sum([np.prod(obs.shape) for obs in spec.observation_specs])  # Total dimensions from all observations
action_dim = spec.action_spec.continuous_size if spec.action_spec.is_continuous() else spec.action_spec.discrete_size

ppo_agent = PPO(state_dim, action_dim, lr_actor, lr_critic, gamma, K_epochs, eps_clip, has_continuous, action_std)

for episode in range(3):
  env.reset()
  decision_steps, terminal_steps = env.get_steps(behavior_name)
  tracked_agent = -1 # -1 indicates not yet tracking
  done = False # For the tracked_agent
  episode_rewards = 0 # For the tracked_agent

  while not done:
    # Track the first agent we see if not tracking
    # Note : len(decision_steps) = [number of agents that requested a decision]
    if tracked_agent == -1 and len(decision_steps) >= 1:
      tracked_agent = decision_steps.agent_id[0]
    # Generate an action for all agents
    obs = decision_steps.obs  # Assuming this is a list of arrays
    flattened_obs = torch.cat([torch.FloatTensor(o).flatten() for o in obs])
    state = flattened_obs.unsqueeze(0)
    action = ppo_agent.select_action(state)
    action = action.reshape(1, -1)
    # Set the actions
    continuous_actions = ActionTuple(continuous=action)
    env.set_actions(behavior_name, continuous_actions)
    # Move the simulation forward
    env.step()
    # Get the new simulation results
    decision_steps, terminal_steps = env.get_steps(behavior_name)
    if tracked_agent in decision_steps: # The agent requested a decision
      episode_rewards += decision_steps[tracked_agent].reward
    if tracked_agent in terminal_steps: # The agent terminated its episode
      episode_rewards += terminal_steps[tracked_agent].reward
      done = True
  print(f"Total rewards for episode {episode} is {episode_rewards}")



env.close()
print("Closed environment")
