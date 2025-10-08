import { Routes, Route } from "react-router-dom";
import JEGASolutionsLanding from "./components/JEGASolutionsLanding";
import PaymentSuccess from "./pages/PaymentSuccess";
import "./index.css";

function App() {
  return (
    <Routes>
      <Route path="/" element={<JEGASolutionsLanding />} />
      <Route path="/payment-success" element={<PaymentSuccess />} />
    </Routes>
  );
}

export default App;