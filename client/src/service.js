import axios from 'axios';

// הגדרת כתובת ה-API כברירת מחדל (Config Defaults)
// ודאי שהפורט תואם למה שמופיע לך בטרמינל של ה-API
axios.defaults.baseURL = process.env.REACT_APP_API_URL;

// הוספת Interceptor לתפיסת שגיאות ורישומן ללוג
axios.interceptors.response.use(
  response => response, // אם התגובה תקינה, פשוט החזר אותה
  error => {
    // אם יש שגיאה (סטטוס שונה מ-2xx), היא תירשם כאן ללוג
    console.error("API Error:", error.response ? error.response.data : error.message);
    return Promise.reject(error); // המשך העברת השגיאה כדי שהקוד יוכל לטפל בה במידת הצורך
  }
);

export default {
  // שליפת כל המשימות
  getTasks: async () => {
    const result = await axios.get("/items");    
    return result.data;
  },

  // הוספת משימה חדשה
  addTask: async (name) => {
    console.log('addTask', name);
    // שליחת אובייקט עם שם המשימה וסטטוס ברירת מחדל (לא בוצע)
    const result = await axios.post("/items", { name: name, isComplete: false });
    return result.data;
  },

  // עדכון סטטוס משימה (סימון כבוצע/לא בוצע)
  setCompleted: async (id, isComplete) => {
    console.log('setCompleted', { id, isComplete });
    // שימי לב שאנחנו שולחים את ה-ID ב-URL ואת הנתונים ב-Body
    // בהתאם ל-MapPut שהגדרנו ב-API
    const result = await axios.put(`/items/${id}`, { isComplete: isComplete });
    return result.data;
  },

  // מחיקת משימה
  deleteTask: async (id) => {
    console.log('deleteTask');
    const result = await axios.delete(`/items/${id}`);
    return result.data;
  }
};