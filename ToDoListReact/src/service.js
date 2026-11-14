import axios from "axios";

axios.defaults.baseURL = "http://localhost:5182";
axios.interceptors.response.use(
  (response) => response,
  (error) => {
    console.error("API Error:", error);
    return Promise.reject(error);
  },
);

export default {
  getTasks: async () => {
    const result = await axios.get("/items");
    return result.data;
  },

  addTask: async (name) => {
    const result = await axios.post("/items", {
      name: name,
      isComplete: false,
    });
    return result.data;
  },

  setCompleted: async (id, isComplete) => {
    const tasks = await axios.get("/items");
    const task = tasks.data.find((task) => task.id === id);

    if (!task) {
      throw new Error("Task not found");
    }

    await axios.put(`/items/${id}`, {
      name: task.name,
      isComplete: isComplete,
    });

    return {};
  },

  deleteTask: async (id) => {
    await axios.delete(`/items/${id}`);
  },
};
