import React from "react";
import NumberFormat from "react-number-format";
import PropTypes from "prop-types";
import * as moment from "moment";

import TextField from "@material-ui/core/TextField";
import { PRIMARYCOLOR } from "../../../constants/utils";
import Wrapper from "../Wrapper/Wrapper.component";
import QuoteDetailsBox from "../QuoteDetailsBox";
import { FormControlLabel, InputLabel, NativeSelect } from "@material-ui/core";
import Checkbox from "@material-ui/core/Checkbox";
import Loader from "react-loader-spinner";

const minDate = moment().format("yyyy-MM-DD");
const DISPONIBLE = 0;
const APARTADO = 1;
const VENDIDO = 2;
export const NumberFormatCustom = (props) => {
  const { inputRef, onChange, ...other } = props;
  return (
    <NumberFormat
      {...other}
      getInputRef={inputRef}
      onValueChange={(values) => {
        onChange({
          target: {
            name: props.name,
            value: values.value,
          },
        });
      }}
      thousandSeparator
      isNumericString
      prefix="$"
    />
  );
};

const Unit = ({ state, dispatch }) => {
  const onDateTimeChange = (e) => {
    dispatch({
      type: "SET_QUOTE",
      payload: { "initial-date": e.target.value },
    });
  };


  if (state.isLoading) {
    return (
      <div
        style={{
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
        }}
      >
        {" "}
        <Loader type="ThreeDots" color={PRIMARYCOLOR} />{" "}
      </div>
    );
  }

  return (
    <Wrapper style={{ flexFlow: "column" }}>
      <TextField
        color="secondary"
        required
        id="initialdate"
        name="initialdate"
        label="Fecha inicial"
        type="date"
        onChange={onDateTimeChange}
        value={state.quote["initial-date"]}
        variant="outlined"
        InputLabelProps={{
          shrink: true,
        }}
        style={{
          width: "25em",
          margin: "1em 0px",
          labelRoot: {
            fontSize: "30px !important",
          },
        }}
        InputProps={{ inputProps: { min: minDate } }}
      />
      <div style={{ display: "flex", alignItems: "row" }}>
        <div>
          <InputLabel shrink htmlFor="unidad">
            Unidad
          </InputLabel>
          <NativeSelect
            value={state.quote.unit ? state.quote.unit.id : "-1"}
            onChange={(e) => {
              const selectedUnit = state.quote.units.find(
                (x) => x.id === e.target.value
              );
              dispatch({ type: "SET_QUOTE", payload: { unit: selectedUnit } });
            }}
            name="unidad"
            label="Unidad"
            style={{ margin: "1em 0px", width: "25em" }}
            inputProps={{ "aria-label": "membership", id: "unit-selector" }}
          >
            <option value="-1" disabled>
              Ninguno
            </option>
            <optgroup label="Disponible">
              {state.quote &&
                state.quote.units &&
                state.quote.units
                  .filter((x) => x.isActive && x.status === DISPONIBLE)
                  .sort((a, b) => a.level - b.level || a.number - b.number)
                  .map((u) => (
                    <option key={u.id} value={u.id}>
                      {" "}
                      {u.displayName}
                    </option>
                  ))}
            </optgroup>
            <optgroup label="Apartado">
              {state.quote &&
                state.quote.units &&
                state.quote.units
                  .filter((x) => x.isActive && x.status === APARTADO)
                  .sort((a, b) => a.level - b.level || a.number - b.number)
                  .map((u) => (
                    <option disabled key={u.id} value={u.id}>
                      {" "}
                      {u.displayName}
                    </option>
                  ))}
            </optgroup>
            <optgroup label="Vendido">
              {state.quote &&
                state.quote.units &&
                state.quote.units
                  .filter((x) => x.isActive && x.status === VENDIDO)
                  .sort((a, b) => a.level - b.level || a.number - b.number)
                  .map((u) => (
                    <option disabled key={u.id} value={u.id}>
                      {" "}
                      {u.displayName}
                    </option>
                  ))}
            </optgroup>
          </NativeSelect>
        </div>
        />}
      </div>
      {state.quote.unit && (
        <div
          style={{
            display: "grid",
            gridTemplateColumns: "1fr 1fr",
            gridGap: "1em",
          }}
        >
          <QuoteDetailsBox
            value={state.quote.unit.displayName}
            height="70px"
            title={"Nombre"}
            fontSize={22}
            isText
          />
          <QuoteDetailsBox
            value={state.quote.unit.level > 0 ? state.quote.unit.level : "PB"}
            height="70px"
            title={"Nivel"}
            fontSize={22}
            isText
          />
          <QuoteDetailsBox
            value={`${state.quote.unit.grossArea} m2`}
            height="70px"
            title={"Superficie total"}
            fontSize={22}
            isText
          />
          <QuoteDetailsBox
            value={state.quote.unit.price}
            height="70px"
            title={"Precio de lista"}
            fontSize={22}
          />
        </div>
      )}
    </Wrapper>
  );
};

export default Unit;

Unit.propTypes = {
  dispatch: PropTypes.func.isRequired,
  state: PropTypes.object,
};

NumberFormatCustom.propTypes = {
  inputRef: PropTypes.func.isRequired,
  name: PropTypes.string.isRequired,
  onChange: PropTypes.func.isRequired,
};
