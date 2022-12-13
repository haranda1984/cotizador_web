import React from 'react'
import styled from 'styled-components'
import PropTypes from 'prop-types'
import { MONEYFORMATTER, PRIMARYBLUE, PRIMARYCOLOR } from '../../../constants/utils'

const StyledDetailWrapper = styled.div`
  height: ${p => p.height ? p.height : '92px'};
  display: flex;
  align-items: center;
  justify-content: center;
  border:  ${p => p.active ? PRIMARYBLUE : p.main ? PRIMARYBLUE : PRIMARYCOLOR} 2px solid ;
  color: ${p => p.active ? PRIMARYBLUE : p.main ? 'white' : PRIMARYCOLOR};
  ${p => p.main ? `background-color: ${PRIMARYBLUE}` : ' '};
  border-radius:25px;
  ${p => p.gridArea ? `grid-area: ${p.gridArea}` : ''};
  ${p => p.clicklable ? `cursor: pointer` : ''};
`

const GainPercentageWrapper = styled.div`
  display: flex;
  justify-content: space-between;
`
const QuoteDetailsBox = ({ value, active, fontSize = 32, gridArea, title, subtitle, main, gainPercentage, isText, height, clicklable }) => {
  return <div style={{ gridArea: gridArea }}>
    {gainPercentage ? <GainPercentageWrapper>
      <h3 style={{ marginTop: 0, marginBottom: 0, fontWeight: 'normal' }}>{title}</h3>
      <h3 style={{ marginTop: 0, marginBottom: 0 }}>{`${subtitle} ${(gainPercentage * 100).toFixed(2)}%`}</h3>
    </GainPercentageWrapper>
      : <h3 style={{ marginTop: 0, marginBottom: 0, fontWeight: 'normal' }}>{title}  {subtitle && <strong> {subtitle}</strong>}</h3>}
    {title && <hr />}
    <StyledDetailWrapper active={active} fontSize={fontSize} gridArea={gridArea} main={main} height={height} clicklable={clicklable}>
      <span style={{ fontSize: fontSize }}> {isText ? value : MONEYFORMATTER.format(value)} </span>
    </StyledDetailWrapper>
  </div>
}

export default QuoteDetailsBox

QuoteDetailsBox.propTypes = {
  active: PropTypes.bool,
  main: PropTypes.bool,
  fontSize: PropTypes.number,
  height: PropTypes.string,
  gridArea: PropTypes.string,
  title: PropTypes.string,
  subtitle: PropTypes.string,
  value: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
  gainPercentage: PropTypes.number,
  isText: PropTypes.bool,
}
