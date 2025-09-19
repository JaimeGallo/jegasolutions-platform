import "./AddExtrahour.scss"; // Import your styles
import { FormExtraHour } from "../components/forms/FormExtraHour/FormExtraHour";

const AddExtrahour = () => {
  return (
    <>
      <div>
        <header className="page__header"></header>
        {/* <h2 className="h2addextra">Agregar Horas Extra</h2> */}
        <FormExtraHour />
      </div>
    </>
  );
};

export default AddExtrahour;
