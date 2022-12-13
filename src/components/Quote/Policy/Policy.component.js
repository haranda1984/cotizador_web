import React, { useEffect, useState } from "react";
import PropTypes from "prop-types";
import Wrapper from "../Wrapper/Wrapper.component";
import NativeSelect from "@material-ui/core/NativeSelect";
import TextField from "@material-ui/core/TextField";
import QuoteDetailsBox from "../QuoteDetailsBox";
import { FormGroup, InputAdornment, InputLabel } from "@material-ui/core";
import { PRIMARYCOLOR } from "../../../constants/utils";
import endpoints from "../../../api-service/endpoints";
import Axios from "axios";
import Loader from "react-loader-spinner";

const Policy = ({ state, dispatch }) => {
  const [isloading, setisloading] = useState(true);

  const loadPolicy = async () => {
    const { data } = await Axios.get(
      `${endpoints.PROJECT}/${state.quote.product}/tradepolicies`
    );
    dispatch({ type: "SET_QUOTE", payload: { tradepolicies: data } });
    setisloading(false);
  };

  useEffect(() => {
    loadPolicy();
  }, []);

  const handleSelectChange = (e) => {
    const selectedPolicy = state.quote.tradepolicies.find(
      (x) => x.id === e.target.value
    );


  const handleTextChange = ({ target }) => {
    let value = target.value ? parseInt(target.value) : 0;

    if (value > 100) return;


  return (
    <Wrapper style={{ flexFlow: "column" }}>
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
          value={state.quote.unit.price}
          height="70px"
          title={"Precio de lista"}
          fontSize={22}
        />

        <FormGroup>
          {isloading ? (
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
          ) : (
            <>
              <InputLabel shrink htmlFor="policy">
                Política comercial
              </InputLabel>
              <NativeSelect
                variant="outlined"
                value={
                  state.quote.tradepolicy ? state.quote.tradepolicy.id : "-1"
                }
                onChange={handleSelectChange}
                name="policy"
                label="policy"
                style={{ margin: "1em 0px" }}
                inputProps={{ "aria-label": "membership", id: "unit-selector" }}
              >
                final
                <option value="-1" disabled>
                  Ninguno
                </option>
                {state.quote.tradepolicies
                  .filter((u) => u.isActive)
                  .sort((a, b) => (a.deposit < b.deposit) ? 1 : -1)
                  .map((x) => (
                    <option key={x.id} value={x.id}>
                      {" "}
                      {x.name}{" "}
                    </option>
                  ))}
              </NativeSelect>
            </>
          )}
        </FormGroup>

        <div></div>

        <TextField
          value={state.quote.discount}
          variant="outlined"
          label="Descuento"
          id="descuento"
          type="number"
          onChange={handleTextChange}
          className="no-spin"
          inputProps={{
            className: "no-spin",
            max: 100,
          }}
          InputProps={{
            endAdornment: <InputAdornment position="end">%</InputAdornment>,
          }}
        />

        <QuoteDetailsBox
          value={
            state.quote["discount-price"] ? state.quote["discount-price"] : 0
          }
          height="100%"
          fontSize={22}
        />
      </div>
      <div style={{ marginTop: "20px" }}></div>
      <QuoteDetailsBox
        value={
          state.quote["final-price"]
            ? state.quote["final-price"]
            : state.quote.unit.price
        }
        height="70px"
        title={"Precio final"}
        fontSize={22}
      />
      {state.isCondo && (
        <>
          <div style={{ marginTop: "20px" }}></div>
        </>
      )}
    </Wrapper>
  );
};

export default Policy;

Policy.propTypes = {
  state: PropTypes.object.isRequired,
  dispatch: PropTypes.func.isRequired,
  isLh: PropTypes.bool,
};
