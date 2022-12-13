import React from 'react'
import styled from 'styled-components'
import axios from 'axios'
import { Formik, Field, Form } from 'formik'
import * as Yup from 'yup'
import { TextField } from 'formik-material-ui'
import Snackbar from '@material-ui/core/Snackbar'
import MuiAlert from '@material-ui/lab/Alert'
import Loader from 'react-loader-spinner'
import endpoints from '../../api-service/endpoints'
import { InvyButton } from '../InvyButton/InvyButton.component'
import { PRIMARYCOLOR } from '../../constants/utils'
import { Paper } from '@material-ui/core'

const StyledForm = styled(Form)`
  display: grid;
  grid-gap: 1em;
  width: 20em;
  align-items:center;
`

const CreateUserSchema = Yup.object().shape({
  Email: Yup.string()
    .email('Correo electronico invalido')
    .required('Requerido'),
  Firstname: Yup.string()
    .required('Requerido')
    .max(50, 'Máximo 50 caracteres'),
  Lastname: Yup.string()
    .required('Requerido')
    .max(100, 'Máximo 50 caracteres'),
  Password: Yup.string()
    .required('Requerido')
    .matches(
      /^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$/,
      'La contraseña debe tener mínimo 8 caracteres, 1 carácter especial ( @$!%*#?& ) y 1 número'
    )
})

function Alert (props) {
  return <MuiAlert elevation={6} variant="filled" {...props} />
}

const NewUser = () => {
  const [open, setOpen] = React.useState(false)
  const [error, setError] = React.useState(false)

  const handleClose = (event, reason) => {
    if (reason === 'clickaway') {
      return
    }
    setOpen(false)
    setError(false)
  }

  return (<>
    <Snackbar open={open} autoHideDuration={6000} onClose={handleClose}>
      <Alert onClose={handleClose} severity="success">
          Usuario agregado correctamente
      </Alert>
    </Snackbar>
    <Snackbar open={error} autoHideDuration={10000} onClose={handleClose}>
      <Alert onClose={handleClose} severity="error">
          Ocurrió un error al guardar el usuario, por favor intente de nuevo. Si el error persiste contacte soporte soporte@rootinc.mx
      </Alert>
    </Snackbar>
    <Paper style={{ padding: '3em', display: 'flex', alignItems: 'center', justifyContent: 'center' }} elevation={3}>
      <Formik
        initialValues={{
          Email: '',
          Firstname: '',
          Lastname: '',
          Password: ''
        }}
        validationSchema={CreateUserSchema}
        onSubmit={async (values, actions) => {
          try {
            await axios.post(endpoints.USER, {
              Firstname: values.Firstname,
              Lastname: values.Lastname,
              Email: values.Email,
              Password: values.Password,
              IsActive: true
            })
            setOpen(true)
            actions.resetForm({
              Email: '',
              Password: '',
              Firstname: '',
              Lastname: ''
            })
          } catch (error) {
            setError(true)
            console.log(error)
          }
        }}
      >{({ errors, isSubmitting }) => (
          <StyledForm>
            <h3 style={{ color: PRIMARYCOLOR }}>Nuevo usuario</h3>
            <Field
              component={TextField}
              variant="outlined"
              name="Firstname"
              label="Nombre"
            />
            <Field
              component={TextField}
              label="Apellidos"
              variant="outlined"
              name="Lastname"
            />
            <Field
              component={TextField}
              name="Email"
              variant="outlined"
              type="email"
              label="Email"
            />
            <Field
              component={TextField}
              name="Password"
              variant="outlined"
              type="text"
              label="Contraseña"
            />
            { isSubmitting
              ? <Loader style={{ display: 'flex', justifyContent: 'center' }} type="ThreeDots" color={PRIMARYCOLOR}/>
              : <InverlevyButton type="submit" disabled={isSubmitting}> Agregar usuario</InverlevyButton>}
          </StyledForm>
        )}

      </Formik>
    </Paper>
  </>)
}

export default NewUser
